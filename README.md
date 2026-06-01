<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9.0" />
  <img src="https://img.shields.io/badge/MAUI-Cross--Platform-AC99EA?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET MAUI" />
  <img src="https://img.shields.io/badge/Theme-Food%20%26%20Drink-FF6B35?style=for-the-badge" alt="Food & Drink" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License" />
</p>

<h1 align="center">🍜 Foodie Log&nbsp;&nbsp;<sub>美食记忆本</sub></h1>

<p align="center">
  <strong>Record every delicious moment. A personal food memory diary built with .NET MAUI.</strong>
</p>

---

## 📖 Overview

**Foodie Log** is a cross-platform mobile application built with **.NET MAUI** for the *Food and Drink* theme. It lets you capture and revisit your favourite dining experiences — from a bowl of braised pork belly rice at Grandma's Kitchen to a delicate matcha tiramisu at a hidden café.

Every entry records the dish name, restaurant, a personal review, location, date, and star rating, forming a rich, searchable food diary that lives on your device.

The app follows the **MVVM** architectural pattern with **CommunityToolkit.Mvvm**, uses **XAML** for declarative UI, and integrates with a **mockapi.io** REST backend for real-world networked data access — with local fallback so the app never crashes offline.

> 🎓 This project was developed as part of the *6G6Z0014 – Mobile Computing* module at **Manchester Metropolitan University** (MMU).

---

## 👤 Author

| | |
|---|---|
| **Name** | **yzx** |
| **Module** | 6G6Z0014 – Mobile Computing |
| **Institution** | Manchester Metropolitan University |
| **Assessment** | 1CWK100 – Developing a Cross-Platform Mobile App |

---

## ✨ Features

### 🧱 Architecture & Code Quality
- **MVVM** architecture with **CommunityToolkit.Mvvm** source generators (`[ObservableProperty]`, `[RelayCommand]`)
- **Dependency Injection** via `MauiProgram.Services` for full testability
- Clean separation of concerns: `Models` → `ViewModels` → `Views` → `Services`
- Reusable services layer (`FoodLogService`, `AccessibilityService`, `SpeechService`)
- Fully commented codebase with consistent C# naming conventions

### 🎨 UI / UX & Accessibility
- **XAML-first** UI across multiple pages with compiled bindings
- Warm food-inspired colour palette (creamy backgrounds, tomato-red accents, basil-green)
- **Dark mode** and **Light mode** with `AppThemeBinding`
- **Large text mode** toggled from Settings, applied across all pages
- **Semantic screen reader** support (`SemanticProperties`, `SemanticScreenReader.Announce`)
- WCAG-aligned accessibility practices: heading levels, hints, and readable font sizes

### 📱 Mobile Hardware Integration
| Feature | API |
|---|---|
| Camera | `MediaPicker.Default.CapturePhotoAsync()` |
| Location | `Geolocation.Default.GetLocationAsync()` |
| Geocoding | `Geocoding.Default.GetPlacemarksAsync()` |
| Text-to-Speech | `TextToSpeech.Default.SpeakAsync()` |
| Vibration | `Vibration.Default.Vibrate()` |
| Haptic Feedback | `HapticFeedback.Default.Perform()` |

✅ **6 hardware features** — exceeding the top-grade threshold of 4.

### 🔧 Core Functionality
- Browse food memories in a scrollable card list
- **Full-text search** across name, restaurant, review, location, and tags
- **Add new entries** with dish name, restaurant, review, location, date, and star rating
- Star-rating selector (1⭐–5⭐) with live label feedback
- Date picker limited to today and earlier (no future dates)
- Pull-to-refresh and empty-state guidance

### 🛡 Validation & Error Handling
- **Client-side form validation**: required fields, future-date guard, non-empty checks
- **Visual error panel** with semantic announcement on validation failure
- **Vibration feedback** on invalid submission
- **Haptic feedback** on successful save
- Try/catch around all hardware and network calls — app never crashes on failure
- **mockapi.io networking** with automatic fallback to local in-memory cache

### 🚀 Deployment
- Targets **Android** (emulator & physical device)
- Targets **Windows** (win10-x64)
- Cross-platform project structure with `Platforms/Android/AndroidManifest.xml` permissions

---

## 🏗 Project Structure

```
FoodDrinkApp/
├── App.xaml / App.xaml.cs              # Application entry point
├── AppShell.xaml / AppShell.xaml.cs    # Shell navigation & routing
├── MauiProgram.cs                      # DI container & service registration
├── GlobalXmlns.cs                      # Global XAML namespace definitions
│
├── Models/
│   └── FoodModel.cs                    # Core data model (dish, restaurant, review, rating, date…)
│
├── ViewModels/
│   ├── BaseViewModel.cs                # MVVM base (IsBusy, Title)
│   ├── MainPageViewModel.cs            # Food memory list, search, navigation
│   └── AddEntryPageViewModel.cs        # Form validation, rating, save logic
│
├── Views/
│   ├── MainPage.xaml / .cs             # Home screen — card list of food memories
│   └── AddEntryPage.xaml / .cs         # Add/edit a food memory entry
│
├── Services/
│   ├── FoodLogService.cs               # Data layer — mockapi.io REST + local fallback
│   ├── AccessibilityService.cs         # Large-text font scaling engine
│   ├── SpeechService.cs                # TTS wrapper with Chinese locale preference
│   └── MockApiConfig.cs                # mockapi.io endpoint configuration
│
├── Platforms/
│   └── Android/
│       └── AndroidManifest.xml         # CAMERA, LOCATION, VIBRATE permissions
│
└── Resources/
    ├── Styles/
    │   ├── Colors.xaml                 # Global colour dictionary
    │   └── Styles.xaml                 # Global control styles
    ├── Fonts/                          # Open Sans
    └── Images/                         # App icon & splash assets
```

---

## 🔗 API Integration (mockapi.io)

The app connects to a **mockapi.io** REST API as its primary data source:

| Endpoint | Method | Description |
|---|---|---|
| `/api/v1/foods` | `GET` | Fetch all food entries |
| `/api/v1/foods/:id` | `GET` | Fetch a single entry |
| `/api/v1/foods` | `POST` | Create a new entry |

**Local fallback**: if the endpoint is unreachable or `MockApiConfig.EndpointUrl` is blank, the app serves data from a built-in in-memory cache of sample entries — guaranteed never to crash during demos.

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

> ⚠️ **Build output note**: This project uses `Directory.Build.props` to redirect build outputs to `C:\MauiBuild\FoodDrinkApp\` to avoid path issues with non-ASCII characters.

---

## 🎥 Screencast Demo Checklist

For marking purposes, the screencast covers:

1. ✅ Introduce "Foodie Log" — a Food & Drink themed personal diary
2. ✅ Browse the home screen with food memory cards
3. ✅ Search across name, restaurant, review, location, and tags
4. ✅ Add a new food memory with star rating
5. ✅ Demonstrate form validation (empty fields, future date)
6. ✅ Demonstrate hardware: Camera, Location, Text-to-Speech, Vibration, Haptic Feedback
7. ✅ Show Dark / Light theme toggle and Large Text mode
8. ✅ Walk through key code files: MVVM structure, Models, Services, DI
9. ✅ Show Android + Windows build output
10. ✅ Display GitHub commit history and this README

---

## 📄 License

MIT © yzx — 2025
