<div dir="rtl" lang="fa" align="right">

# راهنمای رابط کاربری Sevart

## هدف سند

این سند الگوهای رابط کاربری موجود، قواعد لازم برای توسعه و محدودیت‌های فعلی `shop-client` را ثبت می‌کند. هر تغییر UI باید با ساختار موجود هماهنگ باشد و حالت‌های فارسی، RTL، Responsive، Dark Theme و SSR را حفظ کند.

## زبان، جهت و فونت

- زبان رابط فارسی است.
- `shop-client/src/index.html` دارای `lang="fa"` و `dir="rtl"` است.
- `body` در `src/styles.scss` با `direction: rtl` و `text-align: right` تنظیم شده است.
- فونت اصلی `Vazirmatn` است و برای Angular Material نیز استفاده می‌شود.
- متن‌ها، پیام‌های خطا و `aria-label`ها باید فارسی باشند.
- مقدارهایی مانند Slug، URL و Color Code می‌توانند در همان کنترل با `dir="ltr"` نمایش داده شوند.
- داخل کد کامنت فارسی جدید اضافه نشود.

## سبک بصری

هویت UI باید این ویژگی‌ها را حفظ کند:

- Premium و مینیمال
- تمیز و محصول‌محور
- رنگ‌های کرم و قهوه‌ای با استفادهٔ محدود از طلایی
- فضای سفید کافی
- گوشه‌های نرم و Rounded
- سایه‌های کنترل‌شده
- انیمیشن‌های کوتاه و غیرمزاحم

فروشگاه نباید شبیه Marketplaceهای شلوغ طراحی شود.

## ساختار استایل‌ها

- استایل سراسری در `src/styles.scss` قرار دارد.
- توکن‌ها در `src/styles/abstracts/_tokens.scss` تعریف می‌شوند.
- استایل مشترک Auth در `src/styles/components/_auth.scss` قرار دارد.
- هر کامپوننت، SCSS مخصوص خود را نگهداری می‌کند.
- نام‌گذاری کلاس‌ها عمدتاً از BEM پیروی می‌کند.
- Tailwind فعال است، اما ساختار فعلی بیشتر بر SCSS کامپوننتی تکیه دارد.
- Angular Material برای فرم‌ها، دکمه‌ها، آیکن‌ها، Select، Dialog و Snackbar استفاده می‌شود.

## توکن‌های طراحی

توکن‌های موجود شامل گروه‌های زیر هستند:

- رنگ برند: `--color-primary`، `--color-primary-hover` و `--color-on-primary`
- سطح‌ها: `--color-background`، `--color-surface` و `--color-surface-elevated`
- متن و Border: `--color-text`، `--color-muted`، `--color-border` و `--color-border-strong`
- فرم‌ها: توکن‌های `--color-input-*`
- دکمه‌ها: توکن‌های `--color-button-*`
- وضعیت‌ها: `--color-danger` و `--color-success`
- Radius: `--radius-md`، `--radius-lg` و `--radius-xl`
- فاصله‌گذاری: `--space-md`، `--space-lg` و `--space-xl`
- سایه‌ها: `--shadow-sm` و `--shadow-md`
- عرض محتوا: `--container-width`

### محدودیت فعلی توکن‌ها

`--space-sm` در چند فایل استفاده شده، اما در `_tokens.scss` تعریف نشده است. این متغیر باید پیش از گسترش استفاده، به مجموعهٔ توکن‌ها اضافه شود.

بعضی رنگ‌ها همچنان به‌صورت Literal در SCSS نوشته شده‌اند. رنگ‌های تکرارشونده باید هنگام تغییر مرتبط به توکن تبدیل شوند؛ Refactor یک‌جای فایل‌های نامرتبط لازم نیست.

## RTL و CSS منطقی

- برای Layout از `margin-inline`، `padding-inline`، `inset-inline`، `border-inline` و `text-align: start/end` استفاده شود.
- استفاده از `left` و `right` فقط زمانی مجاز است که جهت فیزیکی واقعاً بخشی از طراحی باشد.
- کنترل‌هایی مانند Previous/Next باید در RTL از نظر معنا و جهت آیکن بررسی شوند.
- ترتیب بصری نباید فقط با CSS معکوس شود اگر ترتیب DOM روی دسترس‌پذیری اثر منفی دارد.

در کد فعلی ترکیبی از Propertyهای منطقی و فیزیکی وجود دارد؛ این موضوع بدهی فنی است و در فایل مرتبط به‌تدریج اصلاح می‌شود.

## Responsive Design

Breakpoint مرکزی وجود ندارد و Media Queryها در SCSS کامپوننت‌ها تعریف شده‌اند. Breakpointهای مهم فعلی:

| محدوده | کاربرد فعلی |
| --- | --- |
| `1100px` و `1000px` | Home، Header و Category Page |
| `900px` | Cart، Checkout و Admin Layout |
| `800px` و `768px` | Product Detail، Home و Admin Layout |
| `700px` و `600px` | صفحات فروشگاه، Auth، Header و Product Card |
| `480px` و `360px` | موبایل‌های کوچک |

قواعد توسعه:

- طراحی جدید از ابتدا در موبایل و دسکتاپ بررسی شود.
- اسکرول افقی ناخواسته ایجاد نشود.
- دکمه‌های اصلی در موبایل سطح لمس مناسبی داشته باشند.
- فرم‌های چندستونه در عرض کم تک‌ستونه شوند.
- متن و قیمت در اندازه‌های کوچک بریده یا غیرقابل‌خواندن نشوند.

## Header و جست‌وجو

Header فعلی شامل موارد زیر است:

- لوگو و عنوان فروشگاه
- فرم جست‌وجو
- لینک خانه و محصولات
- تغییر Light/Dark Theme
- سبد خرید و Badge تعداد
- ورود و ثبت‌نام

جست‌وجوی Header پیاده‌سازی شده است:

- مقدار ورودی با `FormControl` نگهداری می‌شود.
- Submit کاربر را به `/products?q=...` هدایت می‌کند.
- Query String دوباره در ورودی نمایش داده می‌شود.
- دکمهٔ پاک‌کردن جست‌وجو وجود دارد.

جست‌وجوی صفحهٔ Products در وضعیت فعلی روی Mock Data انجام می‌شود و پس از اتصال کامل محصولات باید به API منتقل شود.

### محدودیت موبایل Header

در عرض حداکثر `600px`، Navigation اصلی پنهان می‌شود، اما منوی موبایل جایگزین وجود ندارد. افزودن Menu Button، پنل موبایل، Focus Management و بستن منو پس از Navigation هنوز انجام نشده است.

## Layout فروشگاه

`MainLayout` شامل Header، محتوای Route و Footer است. صفحات Storefront باید داخل همین Layout باقی بمانند مگر اینکه دلیل معماری مشخصی برای Layout جدا وجود داشته باشد.

الگوهای مشترک صفحات:

- Container با عرض محدود
- عنوان و توضیح صفحه
- Breadcrumb در صفحات داخلی مرتبط
- Grid واکنش‌گرا برای فهرست محصولات
- Empty، Loading و Error State در صفحات داده‌محور

## صفحهٔ خانه

صفحهٔ Home شامل Hero Slider، دسته‌بندی‌ها و محصولات منتخب است.

- Slider دارای Previous، Next، Indicator و Autoplay است.
- Autoplay فقط در Browser اجرا می‌شود.
- دسته‌بندی‌های فعال از API دریافت می‌شوند.
- محصولات منتخب فعلاً از Mock Data می‌آیند.
- تصاویر از Assetهای WebP استفاده می‌کنند.
- برای `prefers-reduced-motion` انیمیشن‌ها کاهش می‌یابند.

## Product Card

`ProductCard` جزء مشترک نمایش محصول است و شامل تصویر، دسته‌بندی، عنوان، قیمت، وضعیت محصول، لینک جزئیات و افزودن به سبد است.

قواعد:

- نسبت تصویر کارت حفظ شود.
- `alt` تصویر از نام محصول ساخته شود.
- وضعیت Focus فقط به Hover وابسته نباشد.
- در Touch Device اطلاعات ضروری بدون Hover قابل مشاهده باشند.
- محصول غیرقابل‌فروش نباید به سبد اضافه شود.
- نمایش Stock یا شمارندهٔ موجودی ممنوع است.

در موبایل و دستگاه‌های Touch، Overlay کارت دائماً قابل مشاهده است و Transformهای Hover محدود می‌شوند.

## صفحهٔ جزئیات محصول

صفحهٔ Product Detail اکنون از API بارگذاری می‌شود و شامل موارد زیر است:

- Loading State
- حالت پیدا نشدن یا خطای دریافت محصول
- تصویر محصول و تصویر پیش‌فرض
- عنوان و قیمت پایه
- گزینه‌های پویا با ترتیب `DisplayOrder`
- پشتیبانی از Select، Radio و Color
- اعتبارسنجی گزینه‌های Required
- انتخاب تعداد
- محاسبهٔ قیمت نهایی بر اساس `PriceAdjustment`
- افزودن محصول همراه گزینه‌های انتخاب‌شده به Cart
- Snackbar برای بازخورد عملیات

Option UI نباید به Color یا Size محدود شود. نمایش کنترل بر اساس `ProductOptionInputType` تعیین می‌شود.

## صفحهٔ دسته‌بندی و محصولات

- Category Page داده را از API دریافت می‌کند.
- Loading، Empty و حالت «پیدا نشد» دارد؛ خطای دریافت و نبود دسته‌بندی فعلاً در یک حالت نمایش داده می‌شوند.
- Cover، عنوان، توضیح و Product Grid را نمایش می‌دهد.
- صفحهٔ Products هنوز از Mock Data استفاده می‌کند و Query جست‌وجو را محلی فیلتر می‌کند.

پس از اتصال کامل Public Product API، ظاهر و Stateهای فعلی حفظ و فقط Data Source جایگزین می‌شود.

## Cart و Checkout

Cart شامل موارد زیر است:

- نمایش گزینه‌های انتخاب‌شده
- قیمت نهایی هر واحد
- افزایش و کاهش تعداد
- حذف یک ردیف
- پاک‌کردن سبد
- مبلغ کل
- Empty State

دو ترکیب متفاوت از گزینه‌های یک محصول باید دو ردیف مستقل نمایش داده شوند.

Checkout فعلی فرم اطلاعات گیرنده و نشانی را نمایش می‌دهد و Validation سمت Client دارد؛ هنوز به Order API متصل نیست. UI نباید پرداخت یا ثبت سفارش موفق را پیش از پاسخ معتبر Backend قطعی نشان دهد.

## پنل مدیریت

پنل مدیریت Layout مستقل دارد و در SSR به‌صورت Client-rendered اجرا می‌شود.

صفحات فعلی:

- Dashboard اولیه
- فهرست و فرم Category
- فهرست و فرم Product

الگوهای Admin:

- List Page دارای Header، Action اصلی، Loading، Empty و عملیات هر ردیف باشد.
- عملیات حذف و تغییر وضعیت بازخورد واضح داشته باشند.
- عملیات مخرب از Confirm Dialog استفاده کنند.
- فرم‌ها با Reactive Forms و پیام خطای نزدیک به فیلد ساخته شوند.
- Submit در حالت نامعتبر یا در حال ارسال غیرفعال شود.
- Slug، URL و Color Code می‌توانند LTR باشند.

### فرم Product

فرم Product شامل اطلاعات پایه، Category، قیمت، Status، DisplayOrder، تصاویر و گزینه‌هاست.

برای گزینه‌های پویا:

- هر Option یک Card مستقل دارد.
- Definition گزینه از لیست Backend انتخاب می‌شود.
- Required و DisplayOrder قابل تنظیم‌اند.
- Valueها شامل Label، Value، PriceAdjustment، ColorCode، DisplayOrder و Active هستند.
- ColorCode فقط برای Input Type نوع Color نمایش داده شود.
- حذف Option یا Value موجود باید با رفتار Backend و Deactivate هماهنگ باشد.

آپلود واقعی فایل تصویر هنوز پیاده‌سازی نشده و یکی از Featureهای اصلی بعدی پروژه است. UI آینده باید انتخاب فایل، Preview، Progress، Error، ترتیب و انتخاب تصویر اصلی را پوشش دهد.

## Theme روشن و تیره

- Theme با `data-theme` روی عنصر `<html>` کنترل می‌شود.
- انتخاب کاربر در `localStorage` با کلید `shop-theme` ذخیره می‌شود.
- در نبود انتخاب ذخیره‌شده، `prefers-color-scheme` استفاده می‌شود.
- رنگ یا Surface جدید باید در هر دو Theme بررسی شود.
- Component جدید نباید با رنگ Literal فقط در یکی از Themeها خوانا باشد.

## وضعیت‌های UI

هر صفحهٔ متصل به داده باید در صورت نیاز این حالت‌ها را داشته باشد:

1. Initial/Idle
2. Loading
3. Success
4. Empty
5. Error
6. Submitting یا Mutating

وضعیت فعلی:

- Loading در Category Page، Product Detail و Listهای Admin وجود دارد.
- Empty State در Products، Category، Cart، Checkout و Listهای Admin دیده می‌شود.
- خطاهای Validation فرم با `mat-error` نمایش داده می‌شوند.
- Loading و Error Stateها هنوز به یک Component یا الگوی مشترک تبدیل نشده‌اند.
- Auth و Checkout هنوز به API واقعی متصل نیستند؛ Success State آن‌ها نهایی نیست.

## فرم‌ها و اعتبارسنجی

- از Reactive Forms استفاده شود.
- پیام خطا بعد از تعامل کاربر یا Submit نامعتبر نمایش داده شود.
- Required، Length، Pattern و Range در Client و Backend هماهنگ باشند.
- Validation سمت Client جایگزین Validation سمت Server نیست.
- هنگام Submit تکراری از ارسال چندباره جلوگیری شود.
- خطای Server در سطح فیلد یا فرم قابل مشاهده باشد.
- برای موبایل از `type`، `inputmode` و `autocomplete` مناسب استفاده شود.

## دسترس‌پذیری

موارد موجود:

- `aria-label` برای دکمه‌های آیکنی
- `alt` برای تصاویر محصول
- کنترل Keyboard و ARIA در Slider
- Breadcrumb با Label
- `prefers-reduced-motion` در چند بخش
- Focus State در اجزای تعاملی اصلی

موارد باقی‌مانده:

- Skip Link به محتوای اصلی
- Focus Management منوی موبایل و Dialogهای پیچیده
- بررسی Contrast در هر دو Theme
- تست کامل Keyboard Navigation
- اعلام مناسب Loading، Error و Success برای Screen Reader

## SEO و تصویر

- صفحات عمومی Title، Description، Open Graph و Canonical دارند.
- تصویر محصول باید `alt` معنادار داشته باشد.
- تصاویر پایین صفحه Lazy-load شوند.
- ابعاد تصویر تا حد امکان مشخص باشد تا Layout Shift کم شود.
- تصویر پیش‌فرض فقط هنگام نبود تصویر واقعی استفاده شود.
- پس از پیاده‌سازی Upload، خروجی‌های بهینهٔ WebP/AVIF و Cache Policy بررسی شوند.

## محدودیت‌های فعلی

- منوی موبایل Header وجود ندارد.
- Products و Featured Products هنوز Mock هستند.
- جست‌وجو Server-side نیست.
- `--space-sm` تعریف نشده است.
- بعضی Propertyهای فیزیکی و رنگ‌های Literal باقی مانده‌اند.
- Loading/Error Stateها الگوی مشترک ندارند.
- Auth و Checkout نمایشی‌اند.
- Upload واقعی تصویر وجود ندارد.
- صفحهٔ 404 مستقل وجود ندارد.
- Admin فاقد Authentication و Authorization است.

## چک‌لیست تغییرات UI

پیش از پایان هر تغییر UI بررسی شود:

- [ ] فارسی و RTL حفظ شده است.
- [ ] Light و Dark Theme بررسی شده‌اند.
- [ ] موبایل، تبلت و دسکتاپ بررسی شده‌اند.
- [ ] Keyboard و Focus State قابل‌استفاده‌اند.
- [ ] Loading، Empty، Error و Success موردنیاز پوشش داده شده‌اند.
- [ ] Browser APIها در SSR محافظت شده‌اند.
- [ ] Stock در Storefront نمایش داده نمی‌شود.
- [ ] Optionها generic باقی مانده‌اند.
- [ ] تغییر به فایل‌های مرتبط محدود شده است.
- [ ] Build و تست مرتبط اجرا شده‌اند.

</div>
