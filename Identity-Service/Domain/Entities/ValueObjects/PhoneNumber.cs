using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharedKernel.Domain.Primitives;

namespace Identity_Service.Domain.Entities.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

            if (!IsValidPhoneNumber(phoneNumber))
                throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));

            return new PhoneNumber(FormatPhoneNumber(phoneNumber));
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // مرحله 1: پاک‌سازی اولیه
            var cleanedNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // مرحله 2: اعتبارسنجی فرمت‌های مختلف

            // حالت 1: شماره بین‌المللی (مثلاً +989123456789)
            var internationalRegex = new Regex(@"^\+[1-9]\d{1,14}$");
            if (internationalRegex.IsMatch(cleanedNumber))
            {
                return true;
            }

            // حالت 2: شماره ایرانی بدون کد کشور (مثلاً 09123456789)
            var iranianRegex = new Regex(@"^0[1-9]\d{9}$");
            if (iranianRegex.IsMatch(cleanedNumber))
            {
                return true;
            }

            // اگر هیچ‌کدام از فرمت‌ها مطابقت نداشت، شماره نامعتبر است
            return false;
        }

        private static string FormatPhoneNumber(string phoneNumber)
        {
            // ابتدا شماره را از کاراکترهای اضافه پاک می‌کنیم
            var cleanedNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // اگر شماره با 0 شروع شد (شماره ایرانی)، آن را به فرمت بین‌المللی تبدیل می‌کنیم
            if (cleanedNumber.StartsWith("0"))
            {
                // حذف صفر اول و اضافه کردن کد کشور ایران
                return "+98" + cleanedNumber.Substring(1);
            }

            // اگر شماره با + شروع شد (شماره بین‌المللی)، همانطور که هست برمی‌گردانیم
            if (cleanedNumber.StartsWith("+"))
            {
                return cleanedNumber;
            }

            // در غیر این صورت، فرمت نامشخص است، اما فرض می‌کنیم همان شماره تمیز شده است
            return cleanedNumber;
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}