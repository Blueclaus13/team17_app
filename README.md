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
1.  **Dashboard:**
    * Displays the "Quote of the Day."
    * Shows the current date.
    * Features a daily prompt (e.g., *"What was something good that happened today?"*) to encourage engagement.

2.  **Create Entry:**
    * Users can log a new journal entry.
    * **Data Points:** Date, Mood (Enum), Activity (Enum), and Notes.

3.  **Journal History:**
    * View past entries.
    * Filter history by Mood, Activity, Date, or Keywords.

4.  **Account Page (OAuth Integrated):**
    * Secure login using **Google OAuth**.
    * Displays user profile information (Name, Email, and Profile Picture) retrieved via Google Claims.

## Technology Stack
* **Framework:** ASP.NET Core MVC (.NET 9.0)
* **Authentication:** Google OAuth 2.0
* **Language:** C#
* **Frontend:** Razor Views (HTML/CSS/Bootstrap)
* **Database:** SQL Server / Entity Framework Core
