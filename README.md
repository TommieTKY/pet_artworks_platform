# Pet Artworks Platform

**Pet Artworks Platform** is a creative collaboration web application that bridges the **ArtGallery** and **PawPals** projects.

Built with ASP.NET Core, the platform provides comprehensive management tools for different domains.
**ArtGallery** allows guests to explore artworks, artists, and exhibitions, while registered users can manage gallery content.
**PawPals** enables admins to oversee members and their pets, facilitating connections and CRUD operations. Both applications utilize Entity Framework and MVC architecture for a seamless and user-friendly experience.

The core connection between ArtGallery and PawPals is **pet-related artwork**. Users can explore artworks featuring pets and access detailed pet information associated with each piece. Registered users can also add or remove artworks directly from a pet's detail page, enhancing the interactive experience between art and pet communities.

---

## Roles:

- **Admin**: manage Pet Artworks Platform system.
- **ArtistUser**: manage their profile and artworks.
- **MemberUser**: manage their own pets, following and artworks.
- Regular users can view artist, artworks, exhibitions, pet, owner and connection.

---

## Key Features

### ArtGallery

- **Artists Management**:

  - Add, update, delete, and retrieve artist details.
  - Associate artists with their artworks.

- **Artworks Management**:

  - Add, update, and delete artworks.
  - Associate artworks with exhibitions and pets.

- **Exhibitions Management**:

  - Create, update, and delete exhibitions.
  - Add or remove artworks from exhibitions.
  - View ongoing, past, and future exhibitions.

- **Interactive Features**:

  - Exhibition Status: View exhibitions categorized as past, ongoing, or future.
  - Lightbox2 for enhanced image viewing.
  - Rich text editing with TinyMCE.
  - HTML sanitization for secure input handling.

### PawPals

- **Member Management**:
  - Add, update, delete, and view member profiles.
  - Manage followers and following relationships.
  - View pets owned by members.
- **Pet Management**:
  - Add, update, delete, and view pet profiles.
  - Manage pet owners directly from the pet details page.
  - Add or remove artworks associated with pets.
- **Connection Management**:
  - Add and delete connections between members.

### Core Integration

- **Pet-Related Artworks**:
  - Explore artworks featuring pets and access detailed pet information.
  - Add or remove artworks directly from a pet's detail page.

### Extra features:

- Authentication: Secure authentication for creating, editing, and deleting content.
- Image Management: Upload and display images for artworks and pets.

---

## Roles and Permissions

- **Admin**: Full access to manage the system, including artists, artworks, exhibitions, members, pets and connection.
- **ArtistUser**: Manage their profile and artworks.
- **MemberUser**: Manage their pets, followers, and associated artworks.
- Guest: View artists, artworks, exhibitions, pets, owners, and connections.

---

## Technologies Used

- **Frontend**: Blazor (Server-side)
- **Backend**: ASP.NET Core MVC
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: ASP.NET Core Identity
- **HTML Sanitization**: `Ganss.XSS` library
- **Styling**: Bootstrap
- **Target Framework**: .NET 8
- **Language**: C# 12.0

---

## Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio (recommended for development)
- NuGet packages:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.AspNetCore.Identity`
  - `Ganss.XSS`

---

## To run this project

### 1. Clone the Repository

`git clone https://github.com/TommieTKY/pet_artworks_platform.git`

### 2. Tools > NuGet Package Manager > Package Manager Console

`update-database`

---
