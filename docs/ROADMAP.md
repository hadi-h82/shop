# نقشه راه پروژه

این نقشه راه بر پایهٔ کد فعلی `shop-client/` و مستندات موجود تنظیم شده است. «انجام‌شده» فقط به معنی وجود پیاده‌سازی در کد فعلی است؛ نه لزوماً تکمیل اتصال به سرویس‌های واقعی.

## ۱. وضعیت فعلی

### قابلیت‌های کامل‌شده

| کار | وضعیت | شواهد |
| --- | --- | --- |
| ساختار فروشگاه Angular با Routeهای خانه، محصولات، دسته‌بندی، جزئیات، سبد، تسویه‌حساب و احراز هویت | انجام‌شده | `src/app/app.routes.ts` و `src/app/pages/` |
| Layout مشترک Header، Footer و MainLayout | انجام‌شده | `src/app/layout/` |
| Light/Dark Theme با ذخیرهٔ انتخاب کاربر | انجام‌شده | `src/index.html` و `src/app/core/services/theme.ts` |
| سبد خرید محلی با افزایش، کاهش، حذف و محاسبهٔ مبلغ | انجام‌شده | `src/app/core/services/cart.ts` و `src/app/pages/cart/` |
| SSR، client hydration و prerender مسیرهای فعلی | انجام‌شده | `src/main.server.ts`، `src/server.ts` و `src/app/app.routes.server.ts` |
| SEO پایه شامل title، description، Open Graph و canonical | انجام‌شده | `src/app/core/services/seo.ts` و صفحه‌های Home، Products، CategoryProducts و ProductDetail |
| اعتبارسنجی سمت کاربر فرم‌های auth و checkout | انجام‌شده | `src/app/pages/auth/` و `src/app/pages/checkout/` |
| تست‌های واحد اولیه برای کامپوننت‌ها و سرویس‌ها | انجام‌شده | فایل‌های `*.spec.ts` در `src/app/` |

### قابلیت‌های نمایشی یا مبتنی بر Mock

| کار | وضعیت | شواهد |
| --- | --- | --- |
| نمایش محصول و دسته‌بندی | انجام‌شده، مبتنی بر Mock | `src/app/core/mock-data/` و صفحه‌های محصول |
| تولید مسیرهای prerender برای محصول و دسته‌بندی | انجام‌شده، مبتنی بر Mock | `src/app/app.routes.server.ts` |
| سبد خرید | انجام‌شده، فقط محلی | `CartService` داده را با کلید `shop-cart` در `localStorage` نگه می‌دارد |
| ثبت سفارش | نمایشی / برنامه‌ریزی‌نشده در کد | `src/app/pages/checkout/checkout.ts` فقط داده را در `console.log` ثبت می‌کند |
| ورود، ثبت‌نام و بازیابی رمز | نمایشی / برنامه‌ریزی‌نشده در کد | فایل‌های `src/app/pages/auth/*/*.ts` فقط دادهٔ معتبر را در `console.log` ثبت می‌کنند |
| جست‌وجو | پیاده‌سازی‌نشده | input در `src/app/layout/header/header.html` منطق یا binding ندارد |

### مشکلات و بدهی‌های فنی قابل مشاهده

| مورد | وضعیت | شواهد |
| --- | --- | --- |
| canonical URLهای فعلی هنوز `sevart.ir` هستند | برنامه‌ریزی‌شده · P0 | ثابت `SITE_URL` در صفحه‌های Home، Products، CategoryProducts و ProductDetail |
| منوی موبایل وجود ندارد و navigation در عرض کوچک پنهان می‌شود | برنامه‌ریزی‌شده · P0 | `src/app/layout/header/header.scss` |
| حالت loading برای صفحه، داده و فرم وجود ندارد | برنامه‌ریزی‌شده · P1 | `docs/UI_GUIDELINES.md` و قالب‌های فعلی |
| `--space-sm` استفاده شده ولی تعریف نشده است | برنامه‌ریزی‌شده · P1 | `src/styles/abstracts/_tokens.scss` و استایل‌های Header/Auth/Footer |
| استفادهٔ هم‌زمان از CSS logical و چپ/راست فیزیکی و رنگ‌های literal | برنامه‌ریزی‌شده · P2 | `src/app/layout/header/header.scss` و `src/app/shared/ui/product-card/product-card.scss` |
| لایهٔ دریافت داده/API وجود ندارد و `Product` service خالی است | برنامه‌ریزی‌شده · P0 | `src/app/core/services/product.ts` و مصرف مستقیم Mockها |

## ۲. فاز اول: تثبیت Frontend

| کار | وضعیت | اولویت |
| --- | --- | --- |
| اصلاح canonical و Open Graph URLهای ثابت از `sevart.ir` به `noviraone.ir` | برنامه‌ریزی‌شده | P0 |
| رفع ناهماهنگی‌های ثبت‌شده در `docs/UI_GUIDELINES.md`، از جمله تعریف `--space-sm` و کاهش استفادهٔ mixed از جهت فیزیکی/منطقی | برنامه‌ریزی‌شده | P1 |
| تکمیل responsive Header و افزودن منوی موبایل برای navigation پنهان‌شده | برنامه‌ریزی‌شده | P0 |
| تکمیل حالت‌های loading، empty و error برای داده‌ها و عملیات فرم | برنامه‌ریزی‌شده | P1 |
| بازبینی SEO و SSR پس از اصلاح دامنه و مسیرهای prerender | برنامه‌ریزی‌شده | P0 |
| افزودن و تکمیل تست‌های ضروری برای جریان‌های سبد، Routeها، فرم‌ها، تم و SSR-safe بودن | برنامه‌ریزی‌شده | P1 |

## ۳. فاز دوم: طراحی Backend

| کار | وضعیت | اولویت |
| --- | --- | --- |
| ایجاد Backend با ASP.NET Core Web API | برنامه‌ریزی‌شده | P0 |
| ایجاد و پیکربندی SQL Server | برنامه‌ریزی‌شده | P0 |
| طراحی قرارداد API | نیازمند تصمیم | — |
| طراحی مدل‌های محصول، دسته‌بندی، کاربر، سبد خرید و سفارش | نیازمند تصمیم | — |
| تعیین سازوکار احراز هویت و سطح دسترسی | نیازمند تصمیم | — |
| تعیین سیاست مدیریت خطا، pagination، فیلتر و جست‌وجو | نیازمند تصمیم | — |

## ۴. فاز سوم: اتصال Frontend به Backend

| کار | وضعیت | اولویت |
| --- | --- | --- |
| جایگزینی `MOCK_PRODUCTS` و `MOCK_CATEGORIES` با API | برنامه‌ریزی‌شده | P0 |
| اتصال فهرست و جزئیات محصول و دسته‌بندی به API | برنامه‌ریزی‌شده | P0 |
| اتصال احراز هویت و فرم‌های auth به API | برنامه‌ریزی‌شده | P0 |
| ثبت سبد خرید و سفارش از طریق API | برنامه‌ریزی‌شده | P0 |
| مدیریت loading، error و retry برای درخواست‌ها | برنامه‌ریزی‌شده | P1 |
| تنظیم environment و URLهای API | نیازمند تصمیم | — |
| تغییر تولید پارامترهای prerender از Mock به دادهٔ سازگار با Backend | برنامه‌ریزی‌شده | P1 |

## ۵. فاز چهارم: امکانات فروشگاهی

| کار | وضعیت | اولویت |
| --- | --- | --- |
| اتصال درگاه پرداخت | نیازمند تصمیم | — |
| تعریف و اتصال روش ارسال | نیازمند تصمیم | — |
| مدیریت موجودی | نیازمند تصمیم | — |
| وضعیت و پیگیری سفارش | برنامه‌ریزی‌شده | P1 |
| پنل مدیریت | نیازمند تصمیم | — |
| حساب کاربری و سوابق سفارش‌ها | برنامه‌ریزی‌شده | P1 |

## ۶. فاز پنجم: انتشار نهایی

| کار | وضعیت | اولویت |
| --- | --- | --- |
| اجرای و بررسی Production Build و SSR | برنامه‌ریزی‌شده | P0 |
| بررسی امنیت و عملکرد | برنامه‌ریزی‌شده | P1 |
| تنظیم محیط Backend و Database برای استقرار | نیازمند تصمیم | — |
| تنظیم دامنه، SSL و متغیرهای محیطی | نیازمند تصمیم | — |
| مانیتورینگ، logging و backup | نیازمند تصمیم | — |

## پیشنهاد قدم بعدی

**P0 — مقدار ثابت `SITE_URL` را در چهار فایل `src/app/pages/home/home.ts`، `src/app/pages/products/products.ts`، `src/app/pages/category-products/category-products.ts` و `src/app/pages/product-detail/product-detail.ts` از `https://sevart.ir` به `https://noviraone.ir` تغییر دهید.**
