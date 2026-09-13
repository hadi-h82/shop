# راهنمای مشارکت در پروژه

## محدوده و ساختار مخزن

- ریشهٔ مخزن: `D:\projects\shop`
- برنامهٔ Angular در `shop-client/` قرار دارد؛ تمام دستورات npm را از همین پوشه اجرا کنید.
- این پروژه یک فروشگاه فارسی و راست‌به‌چپ (RTL) برای فروش لوازم قنادی است.
- ساختار `src/` را حفظ کنید:
  - `src/app/core/`: مدل‌ها، سرویس‌ها و داده‌های mock
  - `src/app/layout/`: اجزای چیدمان شامل header، footer و main-layout
  - `src/app/pages/`: صفحات و جریان‌های کاربری فروشگاه
  - `src/app/shared/ui/`: اجزای رابط کاربری قابل‌استفادهٔ مجدد
  - `src/styles/`: توکن‌ها و استایل‌های مشترک SCSS
- SSR از مسیرهای `src/main.server.ts`، `src/server.ts`، `src/app/app.config.server.ts` و `src/app/app.routes.server.ts` پیکربندی شده است.

## فناوری‌ها و نسخه‌ها

- Angular `22.0.x`، Angular CLI و Build `22.0.5`
- Angular Material و CDK `22.0.4`
- Angular SSR `22.0.5` با Express `5.1.0`
- TypeScript `~6.0.2` و RxJS `~7.8.0`
- SCSS، Tailwind CSS `4.3.2` و Vazirmatn `33.0.3`
- npm `11.16.0`
- تست واحد: Vitest `4.0.8`

## دستورات

این دستورها را از `shop-client/` اجرا کنید:

```bash
npm start
npm run build
npm run watch
npm test
npm run serve:ssr:shop-client
```

- `npm start` سرور توسعه را با پیکربندی development اجرا می‌کند.
- `npm run build` ساخت production با خروجی server/SSR را می‌سازد و باید پس از هر تغییر کد اجرا شود.
- پس از تغییرات، خطاهای build را برطرف کنید و نتیجه را گزارش دهید.

## قواعد پیاده‌سازی

- RTL، زبان فارسی، واکنش‌گرایی موبایل و Dark Theme جزو الزامات پایه‌اند؛ هر تغییر UI باید هر چهار مورد را حفظ کند.
- از جهت‌دهی منطقی CSS مانند `margin-inline`، `padding-inline` و `inset-inline` استفاده کنید و از فرض چپ‌به‌راست در layout پرهیز کنید.
- Dark Theme موجود را حفظ و برای رنگ‌ها و سطوح جدید، حالت تیرهٔ مناسب اضافه کنید.
- پروژه SSR است: به `window`، `document`، `localStorage`، `sessionStorage` و سایر APIهای مرورگر فقط در مرورگر دسترسی بگیرید. از `PLATFORM_ID` و `isPlatformBrowser` استفاده کنید؛ برای DOM، `DOCUMENT` را inject کنید.
- الگوهای Angular 22 موجود را ادامه دهید: کامپوننت‌های standalone، `inject()`، `signal()` و lazy loading با `loadComponent` در جاهای مناسب. با نام‌گذاری فعلی کلاس‌ها و فایل‌ها هماهنگ بمانید (مانند `home.ts`، `home.html` و `home.scss`).
- ساختار موجود، نام پوشه‌ها، مسیرهای route و قراردادهای نام‌گذاری را بدون نیاز روشن تغییر ندهید.
- فقط فایل‌های مرتبط با درخواست را تغییر دهید؛ از بازنویسی یا قالب‌بندی گستردهٔ فایل‌های نامرتبط خودداری کنید.

## مستندات

- مستندات تکمیلی را در پوشهٔ `docs/` در ریشهٔ مخزن قرار دهید.
- برای تغییرات کوچک، از ایجاد مستندات غیرضروری پرهیز کنید؛ برای تصمیم‌ها، قراردادها یا راه‌اندازی‌های مهم، فایل مناسب در `docs/` اضافه یا به‌روزرسانی کنید.
