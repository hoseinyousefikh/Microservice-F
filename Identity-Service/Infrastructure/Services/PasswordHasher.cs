using Identity_Service.Application.Common.Services;
// این using صحیح است. به کلاس‌های استاتیک داخل BCrypt.Net اشاره دارد.
using static BCrypt.Net.BCrypt;

namespace Identity_Service.Infrastructure.Services
{
    /// <summary>
    /// پیاده‌سازی سرویس هش کردن پسورد با استفاده از الگوریتم BCrypt.
    /// این سرویس مسئولیت هش کردن پسوردهای جدید و اعتبارسنجی پسوردهای ورودی با هش‌های ذخیره شده را بر عهده دارد.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// یک پسورد متنی (plaintext) را با استفاده از الگوریتم BCrypt هش می‌کند.
        /// </summary>
        /// <param name="password">پسورد متنی برای هش کردن.</param>
        /// <returns>رشته هش شده پسورد که شامل salt و الگوریتم است.</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));
            }

            // از متد EnhancedHashPassword برای کنترل workFactor استفاده کنید
            // workFactor عددی بین 4 تا 31 است. مقدار پیش‌فرض 11 یا 12 است.
            // مقدار بالاتر یعنی امنیت بیشتر اما پردازش کندتر.
            string hashedPassword = EnhancedHashPassword(password, workFactor: 12);

            return hashedPassword;
        }

        /// <summary>
        /// یک پسورد متنی (plaintext) را با یک هش ذخیره شده مقایسه می‌کند تا صحت آن را تأیید کند.
        /// </summary>
        /// <param name="password">پسورد متنی ورودی از کاربر.</param>
        /// <param name="hashedPassword">هش ذخیره شده در دیتابیس.</param>
        /// <returns>
        /// اگر پسورد ورودی با هش مطابقت داشته باشد، true برمی‌گرداند، در غیر این صورت false.
        /// </returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            {
                return false;
            }

            // متد Verify به طور خودکار workFactor را از هش ذخیره شده می‌خواند
            // و نیازی به مشخص کردن آن ندارد.
            bool isValid = Verify(password, hashedPassword);

            return isValid;
        }
    }
}