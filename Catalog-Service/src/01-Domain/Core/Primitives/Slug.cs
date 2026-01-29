using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalog_Service.src._01_Domain.Core.Primitives
{
    public class Slug : ValueObject
    {
        public string Value { get; }

        private Slug(string value)
        {
            Value = value;
        }

        public static Slug Create(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Slug title cannot be empty", nameof(title));

            string slug = GenerateSlug(title);
            return new Slug(slug);
        }

        public static Slug FromString(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug cannot be empty", nameof(slug));

            if (!IsValidSlug(slug))
                throw new ArgumentException("Invalid slug format", nameof(slug));

            return new Slug(slug);
        }

        public static bool TryFromString(string slug, out Slug result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(slug) || !IsValidSlug(slug))
                return false;

            result = new Slug(slug);
            return true;
        }

        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            // نرمال‌سازی متن و حذف علامت‌ها
            string slug = title.Normalize(NormalizationForm.FormD);

            var slugBuilder = new StringBuilder();

            foreach (char c in slug)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

                // قبول کردن:
                // 1. حروف همه زبان‌ها (Letter)
                // 2. اعداد همه زبان‌ها (DecimalDigitNumber)
                // 3. فاصله‌ها (SpaceSeparator)
                // 4. خط‌تیره و زیرخط
                if (category == UnicodeCategory.LowercaseLetter ||
                    category == UnicodeCategory.UppercaseLetter ||
                    category == UnicodeCategory.TitlecaseLetter ||
                    category == UnicodeCategory.ModifierLetter ||
                    category == UnicodeCategory.OtherLetter ||
                    category == UnicodeCategory.DecimalDigitNumber ||
                    category == UnicodeCategory.LetterNumber ||
                    category == UnicodeCategory.OtherNumber ||
                    category == UnicodeCategory.SpaceSeparator ||
                    c == '-' || c == '_' || c == '~' || c == '.')
                {
                    slugBuilder.Append(c);
                }
                // حذف علامت‌ها و ترکیب‌ها (Diacritics)
                else if (category == UnicodeCategory.NonSpacingMark ||
                         category == UnicodeCategory.SpacingCombiningMark ||
                         category == UnicodeCategory.EnclosingMark)
                {
                    // علامت‌ها را حذف می‌کنیم (مثل é → e)
                    continue;
                }
                // سایر کاراکترها به فاصله تبدیل می‌شوند
                else
                {
                    slugBuilder.Append(' ');
                }
            }

            slug = slugBuilder.ToString();

            // تبدیل به حروف کوچک
            slug = slug.ToLowerInvariant();

            // جایگزینی فاصله‌ها با خط‌تیره
            slug = Regex.Replace(slug, @"\s+", "-", RegexOptions.Compiled);

            // حذف کاراکترهای نامطلوب از ابتدا و انتها
            slug = Regex.Replace(slug, @"^-+|-+$", "", RegexOptions.Compiled);

            // حذف خط‌تیره‌های تکراری
            slug = Regex.Replace(slug, @"-{2,}", "-", RegexOptions.Compiled);

            // اگر اسلاگ خالی شد، یک اسلاگ تصادفی ایجاد کن
            if (string.IsNullOrEmpty(slug))
                slug = GenerateFallbackSlug();

            return slug;
        }

        private static string GenerateFallbackSlug()
        {
            return $"product-{DateTime.UtcNow.Ticks:x}";
        }

        public static bool IsValidSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            // طول مناسب
            if (slug.Length < 1 || slug.Length > 200)
                return false;

            // نباید با خط‌تیره شروع یا پایان یابد
            if (slug.StartsWith("-") || slug.EndsWith("-"))
                return false;

            // نباید خط‌تیره‌های متوالی داشته باشد
            if (slug.Contains("--"))
                return false;

            // کاراکترهای مجاز:
            // - حروف همه زبان‌ها
            // - اعداد
            // - خط‌تیره
            // - نقطه (برای پسوندها)
            // - زیرخط
            // - تیلدا
            foreach (char c in slug)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);

                bool isValidChar =
                    category == UnicodeCategory.LowercaseLetter ||
                    category == UnicodeCategory.UppercaseLetter ||
                    category == UnicodeCategory.TitlecaseLetter ||
                    category == UnicodeCategory.ModifierLetter ||
                    category == UnicodeCategory.OtherLetter ||
                    category == UnicodeCategory.DecimalDigitNumber ||
                    category == UnicodeCategory.LetterNumber ||
                    category == UnicodeCategory.OtherNumber ||
                    c == '-' || c == '_' || c == '~' || c == '.';

                if (!isValidChar)
                    return false;
            }

            return true;
        }

        // متد کمکی برای نمایش اسلاگ در URL
        public string ToUrlSlug()
        {
            return System.Net.WebUtility.UrlEncode(Value);
        }

        // متد کمکی برای ایجاد اسلاگ از نام فارسی/انگلیسی/روسی/چینی...
        public static Slug CreateFromMultiLanguage(string text, string languageCode = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));

            // برای زبان‌های خاص، تبدیل خاص انجام می‌دهیم
            string processedText = ProcessForLanguage(text, languageCode);
            return Create(processedText);
        }

        private static string ProcessForLanguage(string text, string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                return text;

            return languageCode.ToLowerInvariant() switch
            {
                "fa" => ProcessPersianText(text),      // فارسی
                "ar" => ProcessArabicText(text),       // عربی
                "ru" => ProcessCyrillicText(text),     // روسی
                "zh" => ProcessChineseText(text),      // چینی
                "ja" => ProcessJapaneseText(text),     // ژاپنی
                "ko" => ProcessKoreanText(text),       // کره‌ای
                _ => text
            };
        }

        private static string ProcessPersianText(string text)
        {
            // تبدیل نیم‌فاصله به فاصله معمولی
            text = text.Replace('\u200C', ' ');

            // تبدیل اعداد فارسی به انگلیسی (اختیاری)
            var persianNumbers = "۰۱۲۳۴۵۶۷۸۹";
            var englishNumbers = "0123456789";

            for (int i = 0; i < persianNumbers.Length; i++)
            {
                text = text.Replace(persianNumbers[i], englishNumbers[i]);
            }

            return text;
        }

        private static string ProcessArabicText(string text)
        {
            // حذف علامت‌های عربی
            return text;
        }

        private static string ProcessCyrillicText(string text)
        {
            // تبدیل سیریلیک به لاتین (ترانسلیتریشن ساده)
            var cyrillicToLatin = new Dictionary<char, string>
            {
                {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
                {'е', "e"}, {'ё', "yo"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
                {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
                {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
                {'у', "u"}, {'ф', "f"}, {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"},
                {'ш', "sh"}, {'щ', "shch"}, {'ъ', ""}, {'ы', "y"}, {'ь', ""},
                {'э', "e"}, {'ю', "yu"}, {'я', "ya"},
                {'А', "A"}, {'Б', "B"}, {'В', "V"}, {'Г', "G"}, {'Д', "D"},
                {'Е', "E"}, {'Ё', "Yo"}, {'Ж', "Zh"}, {'З', "Z"}, {'И', "I"},
                {'Й', "Y"}, {'К', "K"}, {'Л', "L"}, {'М', "M"}, {'Н', "N"},
                {'О', "O"}, {'П', "P"}, {'Р', "R"}, {'С', "S"}, {'Т', "T"},
                {'У', "U"}, {'Ф', "F"}, {'Х', "Kh"}, {'Ц', "Ts"}, {'Ч', "Ch"},
                {'Ш', "Sh"}, {'Щ', "Shch"}, {'Ъ', ""}, {'Ы', "Y"}, {'Ь', ""},
                {'Э', "E"}, {'Ю', "Yu"}, {'Я', "Ya"}
            };

            var result = new StringBuilder();
            foreach (char c in text)
            {
                if (cyrillicToLatin.TryGetValue(c, out var latin))
                    result.Append(latin);
                else
                    result.Append(c);
            }

            return result.ToString();
        }

        private static string ProcessChineseText(string text)
        {
            // برای چینی: پین‌یین اضافه کن (در واقعیت باید از کتابخانه استفاده کنی)
            // این یک نمونه ساده است
            return text + "-pinyin";
        }

        private static string ProcessJapaneseText(string text)
        {
            // برای ژاپنی: روماجی اضافه کن
            return text + "-romaji";
        }

        private static string ProcessKoreanText(string text)
        {
            // برای کره‌ای: رومانی‌سازی اضافه کن
            return text + "-romanized";
        }

        // اعتبارسنجی برای فرم‌ها
        public static bool IsValidForInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // طول مناسب برای ورودی کاربر
            if (input.Length < 2 || input.Length > 100)
                return false;

            return IsValidSlug(input);
        }

        // تولید اسلاگ با استفاده از ID در صورت تکراری بودن
        public static Slug CreateWithFallback(string title, int? entityId = null)
        {
            string baseSlug = GenerateSlug(title);

            // اگر ID داریم و نیاز به اضافه کردن آن است
            if (entityId.HasValue)
            {
                return new Slug($"{baseSlug}-{entityId.Value}");
            }

            return new Slug(baseSlug);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Slug slug) => slug?.Value;

        public static bool operator ==(Slug left, Slug right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Slug left, Slug right) => !(left == right);
    }

    // اکستنشن‌متد برای راحتی کار
    public static class SlugExtensions
    {
        public static Slug ToSlug(this string text)
        {
            return Slug.Create(text);
        }

        public static Slug ToSlug(this string text, string languageCode)
        {
            return Slug.CreateFromMultiLanguage(text, languageCode);
        }

        public static bool IsValidSlug(this string text)
        {
            return Slug.IsValidSlug(text);
        }

        public static string ToUrlSafe(this string text)
        {
            return Slug.Create(text).ToUrlSlug();
        }
    }
}