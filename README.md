# Pet Artworks Platform

**Pet Artworks Platform** is a creative collaboration web application that bridges the **ArtGallery** and **PawPals** projects.

Built with **ASP.NET Core**, the platform provides comprehensive management tools for different domains.
**ArtGallery** allows guests to explore artworks, artists, and exhibitions, while registered users can manage gallery content.
**PawPals** enables admins to oversee members and their pets, facilitating connections and CRUD operations. Both applications utilize Entity Framework and MVC architecture for a seamless and user-friendly experience.

The **core connection** between ArtGallery and PawPals is **pet-related artwork**. Users can explore artworks featuring pets and access detailed pet information associated with each piece. Registered users can also add or remove artworks directly from a pet’s detail page, enhancing the interactive experience between art and pet communities.

---

## Features - ArtGallery

• Artists Management: View, find, add, update, and delete artists.

• Artworks Management: View, find, add, update, and delete artworks.

• Exhibitions Management: View, find, add, update, and delete exhibitions.

• Artwork-Artist Association: Update the artist from artworks.

• Artwork-Exhibition Association: Add and remove artworks from exhibitions.

• View Artists: View a list of artists with details and associated artists.

• View Artworks: View a list of artworks with details and associated artworks.

• View Exhibitions: View a list of exhibitions with details and associated exhibitions..

• Extra features:

- Authentication: Secure authentication for creating, editing, and deleting content.
- Artwork Image Management: Upload and display images for artworks.
- Exhibition Status: View a list of exhibitions with their status (past, ongoing, future)

## Features - PawPals

### Member Page

- **Index**: Displays a list of all members.
- **Create**: Adds a new member.
- **Details**: Shows detailed information about a specific member.
- **Edit**: Updates member information.
- **Delete**: Deletes an existing member after confirmation.

### Pet Page

- **List (Index)**: Displays all pets.
- **Create**: Adds a new pet.
- **Details**: Shows detailed information about a specific pet.
- **Edit**: Updates pet information.
- **Delete**: Deletes an existing pet after confirmation.

### Connection Page

- **Index**: Displays a list of all connections.
- **Create**: Adds a new connection.
- **Delete**: Deletes an existing connection after confirmation.

## Technologies Used

- .NET 8
- Blazor
- ASP.NET Core MVC
- Entity Framework Core
- Bootstrap

## Project Structure

• Controllers: Contains API controllers for managing artists, artworks, and exhibitions.

• Models: Contains data models representing artists, artworks, and exhibitions.

• Data: Contains the database context and migrations.

• Views: Contains the Razor views for the web pages. These views are used to render the HTML for the web pages.

## API Endpoints

### Artists

• GET /api/Artists/List: Retrieves a list of all artists.

• GET /api/Artists/FindArtist/{ArtistID}: Retrieves details of a specific artist.

• POST /api/Artists/Add: Adds a new artist.

• PUT /api/Artists/Update/{ArtistID}: Updates an existing artist.

• DELETE /api/Artists/Delete/{id}: Deletes an artist.

### Artworks

• GET /api/Artworks/List: Retrieves a list of all artworks.

• GET /api/Artworks/FindArtwork/{ArtworkID}: Retrieves details of a specific artwork.

• POST /api/Artworks/Add: Adds a new artwork.

• PUT /api/Artworks/Update/{ArtworkID}: Updates an existing artwork.

• DELETE /api/Artworks/Delete/{id}: Deletes an artwork.

### Exhibitions

• GET /api/Exhibitions/List: Retrieves a list of all exhibitions.

• GET /api/Exhibitions/FindExhibition/{ExhibitionID}: Retrieves details of a specific exhibition.

• POST /api/Exhibitions/Add: Adds a new exhibition.

• PUT /api/Exhibitions/Update/{ExhibitionID}: Updates an existing exhibition.

• DELETE /api/Exhibitions/Delete/{id}: Deletes an exhibition.

• POST /api/Exhibitions/AddArtwork/{ExhibitionID}: Adds an artwork to an exhibition.

• DELETE /api/Exhibitions/DeleteArtwork/{ExhibitionID}: Removes an artwork from an exhibition.
