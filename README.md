# Mindful Moments - Journal Application

A collaborative .NET web application designed to help users track their daily thoughts, moods, and activities in a simple, aesthetically pleasing interface.

## 👥 Team Members (Team 17)
* Gabriela Elizabeth Rivera
* Nefi Muniz dos Santo
* Claudia Oralia Madrid Ontiveros
* Miguel Ángel Soza González
* Sofia Florylle Sarabia Pantas
* Moshoeshoe Simon Mopeli
* José Israel Carmona Morales

## 🎨 Design & Theme
We have established a unified **"Journal with Pink Colors"** theme to ensure a consistent user experience:
* **Primary Navigation:** Brown (`#5D4037`)
* **Secondary Accents:** Pink (`#F8BBD0` / `#E91E63`)
* **Background:** White (`#FFFFFF`)

## ✨ Key Features

### 1. Authentication
* **Local Account Registration:** Create an account with email and password (secured with BCrypt hashing)
* **Google OAuth Login:** Sign in using your Google account
* **Persistent Sessions:** 60-minute authentication cookies keep you logged in

### 2. Dashboard (Home Page)
* Displays "Quote of the Day" with randomly selected inspirational quotes
* Shows the current date
* Features daily prompts to encourage journaling

### 3. Account Page
* View profile information (Name, Email, Profile Picture)
* Track journal statistics:
  * Total number of entries
  * Weekly streak (days with entries in the past 7 days)
  * Last entry date
* Profile pictures from Google OAuth or auto-generated avatars

### 4. Create Entry
* Log new journal entries with:
  * **Mood:** Happy, Sad, Anxious, Calm, Excited, Angry, Neutral, etc
  * **Activity:** Work, Exercise, Social, Hobby, Rest, Travel, etc
  * **Description:** Free-text notes (max 1000 characters)
* Automatic timestamp in UTC (displayed in your local timezone)
* One journal per user (auto-created on first entry)

### 5. Journal Page
* View all your entries in a responsive card layout (3-column grid)
* Entries displayed newest first
* Each entry shows mood, activity, description, and timestamp

### 6. Edit Entries
* **Same-Day Editing:** Entries can only be edited on the day they were created
* Edit button appears only for today's entries
* Update mood, activity, or description

### 7. Delete Entries
* Confirmation page before deletion
* Permanently removes entry from your journal

## 🛠️ Technology Stack

* **Framework:** ASP.NET Core 9.0 (Hybrid MVC + Blazor Server)
* **Language:** C#
* **Frontend:**
  * Razor Views (MVC for entry forms)
  * Blazor Server (Journal page with real-time rendering)
  * Bootstrap 5 for responsive design
* **Database:** PostgreSQL (hosted on Render)
* **ORM:** Entity Framework Core 9.0 with Npgsql
* **Authentication:**
  * Cookie-based authentication
  * Google OAuth 2.0
  * BCrypt.Net for password hashing
* **Deployment:** Configured for reverse proxy (Render-ready)

## 📋 Prerequisites

* .NET 9.0 SDK
* PostgreSQL database (local or hosted)
* Google OAuth credentials (for Google login)
* Visual Studio Code or Visual Studio 2022

## 🚀 Setup Instructions

### 1. Clone the Repository
```bash
git clone <repository-url>
cd MindfulMomentsApp
```

### 2. Configure Database Connection
Use .NET User Secrets to store your PostgreSQL connection string securely:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=your-host;Database=your-db;Username=your-user;Password=your-password"
```

**Example:**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=mindful_moments;Username=postgres;Password=yourpassword"
```

### 3. Configure Google OAuth (Optional)
If you want to enable Google login:

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable Google+ API
4. Create OAuth 2.0 credentials (Web application)
5. Add authorized redirect URI: `https://localhost:7XXX/signin-google`
6. Store credentials in user secrets:

```bash
dotnet user-secrets set "GOOGLE_CLIENT_ID" "your-client-id.apps.googleusercontent.com"
dotnet user-secrets set "GOOGLE_CLIENT_SECRET" "your-client-secret"
```

### 4. Install Dependencies
```bash
dotnet restore
```

### 5. Apply Database Migrations
```bash
dotnet ef database update
```

### 6. Run the Application
```bash
dotnet run
```

The application will launch at `https://localhost:7XXX` (check console output for exact port).

## 📊 Database Schema

### Users Table
* `UserId` (UUID, Primary Key)
* `Email` (varchar(255), Unique, Required)
* `FirstName` (varchar(100))
* `LastName` (varchar(100))
* `Password` (varchar(255), BCrypt hashed)
* `GoogleId` (varchar(255), for OAuth users)
* `Photo` (varchar(500), profile picture URL)

### Journals Table
* `JournalId` (int, Primary Key)
* `UserId` (UUID, Foreign Key → Users)
* `JournalName` (varchar(200))
* **Relationship:** One-to-One with Users

### Entries Table
* `EntryId` (int, Primary Key)
* `JournalId` (int, Foreign Key → Journals)
* `Mood` (varchar, enum as string)
* `Activity` (varchar, enum as string)
* `Description` (varchar(1000))
* `CreatedDate` (timestamptz)
* `UpdatedDate` (timestamptz, nullable)
* **Constraint:** UpdatedDate must be same day as CreatedDate (or null)
* **Relationship:** Many-to-One with Journals

## 📖 User Guide

### Creating an Account
1. Navigate to the application homepage
2. Click "Sign In" → "Register"
3. Enter your email, password, first name, and last name
4. Click "Register" (you'll be automatically signed in)

**OR** sign in with Google for instant access

### Creating a Journal Entry
1. Sign in to your account
2. Go to the "Journal" page
3. Click "Create New Entry"
4. Select your mood and activity from dropdowns
5. Write your thoughts in the description field
6. Click "Create" to save

### Editing an Entry
1. Go to your Journal page
2. Find the entry you want to edit (must be created today)
3. Click "Edit" button on the entry card
4. Make your changes
5. Click "Save" to update

### Deleting an Entry
1. Go to your Journal page
2. Click "Delete" button on any entry card
3. Confirm deletion on the confirmation page
4. Entry is permanently removed

### Viewing Your Statistics
1. Click on your profile/account
2. View:
   * Total entries created
   * Weekly streak (days with entries in last 7 days)
   * Last entry date

## 🔒 Security Features

* **Password Hashing:** BCrypt with salt for secure password storage
* **HTTPS Enforcement:** All traffic redirected to HTTPS
* **CSRF Protection:** Anti-forgery tokens on all forms
* **Authorization:** `[Authorize]` attribute protects authenticated routes
* **No Response Caching:** Account pages prevent sensitive data caching
* **Database Constraints:** Check constraints enforce same-day editing rule

## 📁 Project Structure

```
MindfulMomentsApp/
├── Controllers/
│   ├── AccountController.cs    # Authentication & user management
│   ├── EntryController.cs      # CRUD operations for entries
│   └── HomeController.cs       # Dashboard & home page
├── Data/
│   ├── AppDbContext.cs         # EF Core database context
│   └── DesignTimeDbContextFactory.cs
├── Models/
│   ├── User.cs                 # User entity
│   ├── Journal.cs              # Journal entity
│   ├── Entry.cs                # Entry entity
│   ├── AccountViewModel.cs     # Account page view model
│   └── Enums/
│       ├── Moods.cs            # Mood enumeration
│       └── Activities.cs       # Activity enumeration
├── Pages/
│   └── Journal.razor           # Blazor page for viewing entries
├── Views/
│   ├── Account/
│   │   ├── Index.cshtml        # Account dashboard
│   │   ├── Register.cshtml     # Registration form
│   │   └── SignIn.cshtml       # Login form
│   ├── Entry/
│   │   ├── Create.cshtml       # Create entry form
│   │   ├── Edit.cshtml         # Edit entry form
│   │   └── Delete.cshtml       # Delete confirmation
│   ├── Home/
│   │   └── Index.cshtml        # Dashboard
│   └── Shared/
│       ├── _Layout.cshtml      # Main layout
│       └── Components/
│           └── EntryCard.razor # Entry display component
├── Helpers/
│   ├── Quotes.cs               # Quote of the day service
│   └── Utils.cs                # Utility functions
├── Migrations/                 # EF Core database migrations
└── wwwroot/                    # Static files (CSS, JS, images)
```