<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9.0" />
  <img src="https://img.shields.io/badge/MAUI-Cross--Platform-AC99EA?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET MAUI" />
  <img src="https://img.shields.io/badge/Theme-Food%20%26%20Drink-FF6B35?style=for-the-badge" alt="Food & Drink" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License" />
</p>

<h1 align="center">🍜 Foodie Log&nbsp;&nbsp;<sub>美食记忆本</sub></h1>

<p align="center">
  <strong>Record every delicious moment.<br/>A cross-platform food memory diary built with .NET MAUI and MVVM.</strong>
</p>

---

## 📖 Overview

**Foodie Log** is a cross-platform mobile application built with **.NET MAUI** for the *Food and Drink* theme. It transforms the way you capture and revisit your dining experiences — from a bowl of braised pork belly rice at Grandma's Kitchen to a delicate matcha tiramisu at a hidden café.

Every entry stores the dish name, restaurant, personal review, photo, GPS location, date, star rating, and optional nutrition facts (calories, protein, carbs, fat). The app connects to a **mockapi.io** REST backend for real-world cloud storage with automatic local fallback — it never crashes offline.

The architecture follows **MVVM** with **CommunityToolkit.Mvvm** source generators, uses **XAML** for declarative UI, and integrates **six mobile hardware** features alongside an **AI computer-vision** scaffolding layer.

> 🎓 This project was developed for *6G6Z0014 – Mobile Computing* at **Manchester Metropolitan University** (MMU).

---

## 👤 Author

| | |
|---|---|
| **Name** | **yzx** |
| **Module** | 6G6Z0014 – Mobile Computing |
| **Institution** | Manchester Metropolitan University |
| **Assessment** | 1CWK100 – Developing a Cross-Platform Mobile App |

---

## 🧭 Development Plan

The project was built incrementally across nine phases, each captured in a dedicated Git commit:

| Phase | Commit Summary | Deliverable |
|---|---|---|
| **1. MVP** | Shell routing + MVVM architecture | Clean `AppShell`, `FoodModel`, `FoodLogService`, DI container |
| **2. Docs** | Professional README | Removed assignment attachments; wrote project overview |
| **3. Cloud** | mockapi.io integration | `GET` / `POST` / `DELETE` REST calls, JSON serialisation, local fallback |
| **4. Hardware** | Camera, GPS, Haptic | `MediaPicker`, `Geolocation`, `Geocoding`, runtime permissions, Android 11+ queries |
| **5. UI/UX** | Accessibility + empty state | 66 `SemanticProperties`, WCAG-aligned labels, Grid overlay layout, `InvertBoolConverter` |
| **6. Gestures + Charts** | Swipe-to-delete + native charts | `SwipeView`, `DeleteMemoryCommand`, `GraphicsView` donut & bar chart drawables, `StatisticsPage` |
| **7. 5-Tab** | Profile, Gallery, Map, AI CV | 5-tab `TabBar`, `ComputerVisionService`, `Map` with `Microsoft.Maui.Controls.Maps`, photo gallery with `GridItemsLayout` |
| **8. Docs** | XML comments + README | Comprehensive C# XML docs on every class, this README |
| **9. Final** | polish | All features verified across Android emulator and Windows build |

---

## ✨ Features

### 🧱 Architecture & Code Quality
- **MVVM** with **CommunityToolkit.Mvvm** source generators (`[ObservableProperty]`, `[RelayCommand]`)
- **Dependency Injection** via `MauiProgram.Services` for Shell DataTemplate pages
- Clean separation: `Models` → `ViewModels` → `Views` → `Services`
- Thread-safe network layer with `Task.Run` offloaded JSON serialisation (prevents ANR)
- Debounced search (300 ms) to avoid flooding the API on keystrokes
- All public methods documented with C# XML `<summary>` tags

### 📱 Mobile Hardware (6 features — exceeds 4-feature top-grade threshold)

| # | Feature | API | Used In |
|---|---|---|---|
| 1 | **Camera** | `MediaPicker.CapturePhotoAsync()` | AddEntryPage + Gallery |
| 2 | **GPS** | `Geolocation.GetLocationAsync()` | AddEntryPage (auto-fill) |
| 3 | **Geocoding** | `Geocoding.GetPlacemarksAsync()` | Address ↔ coordinates |
| 4 | **Text-to-Speech** | `TextToSpeech.SpeakAsync()` | Detail / help screens |
| 5 | **Vibration** | `Vibration.Vibrate()` | Validation error feedback |
| 6 | **Haptic** | `HapticFeedback.Perform()` | Save / delete / AI recognition |

### 🎨 UI / UX & Accessibility (WCAG-aligned)
- **XAML-first** UI across 6+ pages with compiled bindings
- Warm food-inspired palette (creamy backgrounds `#FFF9EF`, tomato-red accents `#D9472B`)
- **Dark mode** + **Light mode** via `AppThemeBinding`
- **Large-text mode** toggled via `AccessibilityService.ApplyFontScale(this)`
- **66 `SemanticProperties`** annotations: `HeadingLevel`, `Description`, `Hint` on every interactive control
- **Screen-reader narration** at key moments: photo capture, AI recognition, save/delete confirmation
- WCAG 2.x alignment documented and referenced

### 🤖 AI Computer Vision (Advanced — 86-100% tier)
- `ComputerVisionService.ClassifyFoodImageAsync(Stream)` — production-ready HTTP scaffold
- Hugging Face / Azure Custom Vision API body + authentication headers (commented, ready to activate)
- MVP simulation: 1.5 s inference latency → deterministic food label → auto-fills dish name
- 12 realistic food labels ("Classic Beef Burger", "Fresh Salmon Sushi", etc.)
- Haptic confirmation when AI label arrives

### 📊 Native Charts (zero external dependencies)
- **Donut chart** — macronutrient ratio (Protein / Carbs / Fat) with arc-path rendering, percentage labels, and three-colour legend
- **Bar chart** — average calories by star rating (1⭐–5⭐) with rounded-rectangle bars and dynamic scaling
- Both rendered via `Microsoft.Maui.Graphics` `IDrawable` + `GraphicsView`

### 🔧 Core Functionality
- **5-tab navigation**: Foodie Log | Gallery | Statistics | Map | Profile
- **Full-text search** across name, restaurant, review, location, tags (debounced)
- **Swipe-to-delete** with `SwipeView` + confirmation dialog + haptic feedback
- **Photo gallery** — 2-column `GridItemsLayout` with `AspectFill` images
- **Interactive map** — `Microsoft.Maui.Controls.Maps` with programmatic restaurant pins
- **User profile** — aggregate stats: total entries, photos, avg rating, most-visited restaurant, total kcal

### 🛡 Validation & Error Handling
- Client-side form validation: required fields, future-date guard
- Visual error panel + `SemanticScreenReader.Announce` + vibration on failure
- `try/catch` around all hardware and network calls — app **never crashes**
- mockapi.io networking with automatic local in-memory fallback

### 🚀 Deployment
- Targets **Android** (emulator + physical) and **Windows** (win10-x64)
- Cross-platform project structure with platform-specific manifests

---

## 🏗 Project Structure

```
FoodDrinkApp/
├── App.xaml / .cs                          # Application entry point
├── AppShell.xaml / .cs                     # Shell with 5-tab TabBar + route registration
├── MauiProgram.cs                          # DI container, .UseMauiMaps(), service registration
├── GlobalXmlns.cs                          # Global XAML namespace definitions
│
├── Models/
│   └── FoodModel.cs                        # Dish, restaurant, review, rating, nutrition, photo, location, date
│
├── ViewModels/
│   ├── BaseViewModel.cs                    # MVVM base (IsBusy, Title)
│   ├── MainPageViewModel.cs                # Food list, debounced search, delete, pull-to-refresh
│   ├── AddEntryPageViewModel.cs            # Form validation, camera, GPS, AI CV, save
│   ├── StatisticsViewModel.cs              # Macro totals + chart data computation
│   ├── GalleryViewModel.cs                 # Photo-only filtered collection
│   ├── MapViewModel.cs                     # Location → coordinate hashing + pin set
│   └── ProfileViewModel.cs                # Aggregate user statistics
│
├── Views/
│   ├── MainPage.xaml / .cs                 # Home — card list + empty state + swipe-to-delete
│   ├── AddEntryPage.xaml / .cs             # Form — camera, GPS, nutrition, AI fill
│   ├── StatisticsPage.xaml / .cs           # Donut + bar charts + totals card
│   ├── GalleryPage.xaml / .cs              # 2-column photo grid with pull-to-refresh
│   ├── MapPage.xaml / .cs                  # Interactive map with restaurant pins
│   ├── ProfilePage.xaml / .cs              # Stats grid + settings
│   ├── DonutChartDrawable.cs               # IDrawable — arc-path donut chart
│   └── BarChartDrawable.cs                 # IDrawable — horizontal bar chart
│
├── Services/
│   ├── FoodLogService.cs                   # mockapi.io CRUD + Task.Run offloading + local cache
│   ├── ComputerVisionService.cs            # AI food classification (HuggingFace scaffold + MVP sim)
│   ├── AccessibilityService.cs             # Large-text font scaling engine
│   ├── SpeechService.cs                    # TTS wrapper with Chinese locale preference
│   └── MockApiConfig.cs                    # mockapi.io endpoint URL
│
├── Converters/
│   └── InvertBoolConverter.cs              # bool inverter for visibility bindings
│
├── Platforms/
│   └── Android/
│       ├── AndroidManifest.xml             # CAMERA, LOCATION, VIBRATE, geo.API_KEY, queries
│       └── MainActivity.cs
│
└── Resources/
    ├── Styles/
    │   ├── Colors.xaml                     # Global colour dictionary
    │   └── Styles.xaml                     # Global control styles (warm food palette)
    ├── Fonts/                              # Open Sans
    └── Images/                             # App icon, splash, + 5 tab icons (SVG)
```

---

## 🔗 API Integration (mockapi.io)

The app connects to a **mockapi.io** REST API:

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/foods` | Fetch all entries |
| `GET` | `/api/v1/foods/:id` | Fetch single entry |
| `POST` | `/api/v1/foods` | Create entry |
| `DELETE` | `/api/v1/foods/:id` | Delete entry |

**Local fallback**: if `MockApiConfig.EndpointUrl` is blank or unreachable, the app serves data from a built-in cache of 4 sample entries — guaranteed never to crash during demos.

To configure your own endpoint, edit `Services/MockApiConfig.cs`:

```csharp
public const string EndpointUrl = "https://YOUR-PROJECT.mockapi.io/api/v1/foods";
```

---

## 🚀 How to Run

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET MAUI workload: `dotnet workload install maui`
- Visual Studio 2022 (recommended) or VS Code with MAUI extension
- Android emulator (API 34+ recommended) or physical device

### Clone & Restore

```bash
git clone <this-repo-url>
cd fooddrink-dead-water627
dotnet restore FoodDrinkApp/FoodDrinkApp.csproj
```

### Build & Run — Android

```powershell
# Build
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net9.0-android --no-incremental

# Run on connected emulator or device
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net9.0-android -t:Run
```

### Build — Windows

```powershell
dotnet build .\FoodDrinkApp\FoodDrinkApp.csproj -f net9.0-windows10.0.19041.0 --no-incremental
```

> ⚠️ **Build output note**: `Directory.Build.props` redirects outputs to `C:\MauiBuild\FoodDrinkApp\` to avoid path issues with non-ASCII characters.

### 🗺️ Map Page — API Key Note

The `Map` tab uses `Microsoft.Maui.Controls.Maps`. On Android a **Google Maps API key** is required for tile rendering. The key placeholder lives in `Platforms/Android/AndroidManifest.xml`:

```xml
<meta-data android:name="com.google.android.geo.API_KEY" android:value="YOUR_GOOGLE_MAPS_API_KEY" />
```

To get a real key: [Google Cloud Console → Maps SDK for Android](https://console.cloud.google.com/google/maps-apis). **Without a real key the map renders as a blank grid — this is expected behaviour and the app does not crash.** Restaurant pins still appear on the grid.

---

## 🎥 Screencast Demo Checklist

For marking purposes the screencast should cover:

| # | Item | Scoring Criterion |
|---|---|---|
| 1 | Introduce "Foodie Log" — Food & Drink theme | Theme compliance |
| 2 | Browse home screen with food-memory cards | Functionality |
| 3 | Search across name, restaurant, review, location | Functionality |
| 4 | Swipe left to delete → confirmation → haptic | Advanced functionality (gestures) |
| 5 | Add a new memory: form, star rating, validation | Validation + Error Handling |
| 6 | 📷 Take Photo → AI auto-fill dish name | Mobile Hardware (Camera) + Advanced (AI CV) |
| 7 | 📍 Get Location → GPS reverse-geocode | Mobile Hardware (Geolocation) |
| 8 | Navigate 5-tab bar: Gallery, Statistics, Map, Profile | UI/UX + Functionality |
| 9 | Statistics page: donut + bar charts | Advanced functionality (charts) |
| 10 | Gallery: photo grid with pull-to-refresh | UI/UX |
| 11 | Map: restaurant pins on interactive map | Mobile Hardware (Maps) |
| 12 | Profile: aggregate user statistics | Functionality |
| 13 | Dark mode toggle + Large-text mode | Accessibility |
| 14 | TalkBack / screen-reader narration examples | Accessibility (WCAG) |
| 15 | Walk through key code: MVVM, Models, Services, DI | Code Quality |
| 16 | Show Android + Windows build output | Deployment |
| 17 | Show GitHub commit history and README | GitHub Usage |

---

## 📄 License

MIT © yzx — 2025
