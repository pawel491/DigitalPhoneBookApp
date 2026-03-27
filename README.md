# AI-Powered Phone Book

A modern web application that functions as a digital phone book, allowing users to manage contact records (Name and Phone Number) seamlessly using natural language prompts.

## Features
* **Natural Language Processing:** Add, edit, delete, and retrieve contacts simply by chatting with the built-in AI assistant. When you ask the AI for a specific contact, the UI automatically scrolls to and highlights it in the list!
* **Standard CRUD:** A clean, traditional UI to manually manage contacts alongside the AI chat.
* **Robust Validation:** Server-side and client-side validation for phone numbers and missing data.
* **Zero-Setup Database:** Automated Entity Framework migrations and data seeding on startup.

## Tech Stack
* **Frontend:** React, TypeScript, Vite, Tailwind CSS
* **Backend:** .NET 9 Web API, Entity Framework Core
* **Database:** PostgreSQL 15
* **AI Integration:** Google Gemini API
* **Infrastructure:** Docker & Docker Compose

## Getting Started

Prerequisites: You only need Docker and Docker Compose installed on your machine.

1. Clone the repository:
```bash
git clone <your-repository-url>
cd <repository-folder>
```

2. Set up Environment Variables:
There is a **.env.example** file included in the root directory. Rename it or copy it to **.env** and provide your Gemini API key like this:
```GEMINI_API_KEY=your_google_gemini_api_key_here```

*(Don't have an API key? You can get a free one from Google AI Studio.)*

3. Run the application:
```docker-compose up -d --build```

4. Access the App:
* Frontend (Main UI): http://localhost:5173
* Backend (API): http://localhost:3000

*Note: The database tables are created automatically upon the first startup.*

## Usage Examples (AI Chat)

You can use the chat interface on the right side of the screen to interact with the phone book. Try prompts like:

* "Add to my phone book John. His phone number is 123456789."
* "Please add a record for Joanna with the number 222333444."
* "What is the phone number for Joanna?"
* "Update John's number to +1 987 654 321."
* "Delete Joanna from my contacts."

## Stopping the App
To stop the containers and clean up the environment, run:

```docker-compose down -v```

---
*This project was developed as a Coding Task for S&P Global Energy internship recruitment.*