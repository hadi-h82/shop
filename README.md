<div dir="rtl" lang="fa" align="right">

# Sevart Frontend

Frontend فروشگاه اینترنتی لوازم قنادی Sevart با Angular 22، SSR و رابط فارسی/RTL.

## فناوری‌ها

- Angular `22.0.x`
- Angular SSR و Express
- Standalone Components
- Signals و RxJS
- Reactive Forms
- Angular Material و CDK
- SCSS و Tailwind CSS
- Vazirmatn
- Vitest

## پیش‌نیازها

- Node.js سازگار با Angular 22
- npm `11.x`
- Backend پروژه برای صفحات متصل به API

نسخهٔ دقیق packageها در `package.json` و `package-lock.json` ثبت شده است.

## نصب

دستورها را از پوشهٔ `shop-client` اجرا کنید:

```bash
npm ci
```

## اجرای Development

```bash
npm start
```

برنامه به‌صورت پیش‌فرض روی آدرس Development مربوط به Angular اجرا می‌شود. API Development در فایل Environment روی مقدار زیر تنظیم شده است:

```text
http://localhost:5090/api
```

برای صفحات متصل به Backend، API باید در دسترس باشد.

## Build

```bash
npm run build
```

پروژه با `outputMode: "server"` ساخته می‌شود و خروجی Browser و Server تولید می‌کند.

در وضعیت فعلی بعضی صفحات Prerender هنگام Build از API داده می‌گیرند؛ اگر API محلی در دسترس نباشد، Build ممکن است شکست بخورد. تصمیم نهایی Render Strategy در `docs/ARCHITECTURE.md` و `docs/ROADMAP.md` ثبت شده است.

## اجرای خروجی SSR

پس از Build:

```bash
npm run serve:ssr:shop-client
```

پورت Express از متغیر `PORT` خوانده می‌شود و مقدار پیش‌فرض آن `4000` است.

## تست

```bash
npm test
```

برای Build پیوسته در محیط Development:

```bash
npm run watch
```

## ساختار اصلی

| مسیر | مسئولیت |
| --- | --- |
| `src/app/admin/` | Layout، Routeها، صفحات و سرویس‌های پنل مدیریت |
| `src/app/core/models/` | مدل‌های Product، Category، Option و Cart |
| `src/app/core/mock-data/` | Mockهای باقی‌ماندهٔ محصولات و Prerender |
| `src/app/core/services/` | سرویس‌های API، Cart، Theme و SEO |
| `src/app/layout/` | Header، Footer و Main Layout |
| `src/app/pages/` | صفحات Storefront، Auth، Cart و Checkout |
| `src/app/shared/` | اجزای مشترک و UI قابل‌استفادهٔ مجدد |
| `src/styles/` | توکن‌ها و استایل‌های مشترک |
| `src/environments/` | تنظیم API برای Development و Production |
| `public/` | تصاویر و Assetهای عمومی |

## Routeهای فروشگاه

| مسیر | صفحه |
| --- | --- |
| `/` | خانه |
| `/products` | همهٔ محصولات و جست‌وجو |
| `/categories/:slug` | محصولات دسته‌بندی |
| `/products/:id` | جزئیات محصول؛ مقدار فعلی پارامتر عملاً Slug است |
| `/cart` | سبد خرید |
| `/checkout` | Checkout نمایشی |
| `/login` | ورود |
| `/register` | ثبت‌نام |
| `/forgot-password` | فراموشی رمز |
| `/reset-password` | بازنشانی رمز |

پنل مدیریت زیر مسیر `/admin` قرار دارد و Routeهای Category و Product را Lazy-load می‌کند.

## وضعیت منابع داده

| بخش | منبع فعلی |
| --- | --- |
| دسته‌بندی‌های Home | API |
| محصولات منتخب Home | Mock |
| صفحهٔ Products | Mock |
| Category Page | API |
| Product Detail | API |
| پنل مدیریت Category/Product | API |
| Cart | `localStorage` |
| Auth و Checkout | فرم محلی و غیرمتصل به Backend |

Mock Data تا زمان حذف کامل وابستگی صفحات و Prerender نباید پاک شود.

## SSR

فایل‌های اصلی SSR:

- `src/main.server.ts`
- `src/server.ts`
- `src/app/app.config.server.ts`
- `src/app/app.routes.server.ts`

Render Mode فعلی:

- `admin/**`: Client
- `products/:id`: Prerender
- `categories/:slug`: Prerender
- سایر Routeها: Prerender

هر دسترسی به `window`، `document`، `localStorage` یا APIهای فقط مرورگر باید SSR-safe باشد.

## Environment

| محیط | API URL |
| --- | --- |
| Development | `http://localhost:5090/api` |
| Production | `https://sevart.ir/api` |

URL سرویس در کد کامپوننت‌ها Hard-code نشود و از Environment خوانده شود.

## قواعد توسعه

- از Standalone Components استفاده کنید.
- از `inject()` و Signals در محل مناسب استفاده کنید.
- از Control Flow جدید Angular مانند `@if` و `@for` استفاده کنید.
- در کد جدید از `*ngFor` استفاده نکنید.
- کامنت فارسی داخل کد اضافه نکنید.
- RTL، فارسی، Responsive Design و Dark Theme را حفظ کنید.
- برای جهت‌دهی CSS از Propertyهای منطقی استفاده کنید.
- Optionهای محصول را به Color یا Size محدود نکنید.
- Stock را در Storefront نمایش ندهید.
- فقط فایل‌های مرتبط با تغییر را ویرایش کنید.
- بعد از هر تغییر، Build یا Test مرتبط را اجرا کنید.

## Cart

Cart با کلید زیر در `localStorage` ذخیره می‌شود:

```text
shop-cart
```

هویت هر ردیف Cart از Product و Optionهای انتخاب‌شده ساخته می‌شود. بنابراین دو انتخاب متفاوت از یک Product، دو ردیف مستقل هستند.

قیمت Client معتبر نهایی نیست؛ هنگام پیاده‌سازی Order، Backend باید قیمت و Optionها را دوباره محاسبه کند.

## مستندات مرتبط

- `../docs/PROJECT_CONTEXT.md`
- `../docs/ARCHITECTURE.md`
- `../docs/UI_GUIDELINES.md`
- `../docs/ROADMAP.md`
- `../AGENTS.md`

## قابلیت‌های مهم بعدی

بر اساس Roadmap فعلی:

1. تثبیت Catalog و Database
2. آپلود و مدیریت واقعی تصویر محصول
3. حذف Mock و اتصال کامل محصولات به API
4. تثبیت SSR و Production Build
5. Authentication، Order، Checkout و Payment

</div>
