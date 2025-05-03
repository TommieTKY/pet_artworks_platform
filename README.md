# Pet Artworks Platform

**Pet Artworks Platform** is a creative collaboration web application that bridges the **ArtGallery** and **PawPals** projects.

Built with ASP.NET Core, the platform provides comprehensive management tools for different domains.
**ArtGallery** allows guests to explore artworks, artists, and exhibitions, while registered users can manage gallery content.
**PawPals** enables admins to oversee members and their pets, facilitating connections and CRUD operations. Both applications utilize Entity Framework and MVC architecture for a seamless and user-friendly experience.

The core connection between ArtGallery and PawPals is **pet-related artwork**. Users can explore artworks featuring pets and access detailed pet information associated with each piece. Registered users can also add or remove artworks directly from a pet's detail page, enhancing the interactive experience between art and pet communities.

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
  - Rich text editing with TinyMCE.
  - HTML sanitization for secure input handling.

### PawPals

- **Member Management**:

  - Create, update, view, and delete member profiles.
  - Manage followers/following relationships.
  - View all pets owned by a specific member.

- **Pet Management**:

  - Create, update, view, and delete pet profiles.
  - Upload and preview pet profile images.
  - Assign or remove pet owners directly from pet details page.
  - Link or remove artworks to/from pets.

- **Connection Management**:

  - Add, view and delete connections between members.
  - Use dropdown list to follow members easily.

- **Interactive Features**:

  - **Role-based access**:
    - `Admin`: Full access across all modules.
    - `MemberUser`: Manage their own profile, pets, and connections.
    - `Guest`: Can only view pets, artworks, and details.

### Core Integration

- **Pet-Related Artworks**:
  - Explore artworks featuring pets and access detailed pet information.
  - Add or remove artworks directly from a pet's detail page.

### Extra features:

- Authentication: Secure authentication for creating, editing, and deleting content.
- Image Management: Upload and display images for artworks and pets.
- Lightbox2 for enhanced image viewing.

---

## Roles and Permissions

- **Admin**: Full access to manage the system, including artists, artworks, exhibitions, members, pets and connection.
- **ArtistUser**: Manage their profile and artworks.
- **MemberUser**: Create/manage member, pets, connections; assign owners and associated artworks to their pets.
- Guest: View artists, artworks, exhibitions, pets and pet details.

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

## 📸 Screenshots

### Home page

![Home Page Interface](/image/home.jpeg)

---

### Pawpals

![Pet Page Interface](/image/pet.jpeg)
![Member Page Interface](/image/member.jpeg)
![Connection Page Interface](/image/connection.jpeg)

---

### Art Gallery

![Artist Page Interface](/image/artist.jpeg)
![Artwork Page Interface](/image/artwork.jpeg)
![Exhibition Page Interface](/image/exhition.jpeg)
