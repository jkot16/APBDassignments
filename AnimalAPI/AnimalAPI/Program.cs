using TutorialRestApi.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<Animal> animals = new List<Animal>
{
    new Animal
    {
        Id = 1,
        Name = "Krzysiu",
        Category = "dog",
        Weight = 20,
        FurColor = "brown",
        Visits = new List<Visit>
        {
            new Visit { Id = 1, Date = DateTime.Parse("2025-04-15 09:30:00", CultureInfo.InvariantCulture), Description = "General Checkup", Price = 50 }
        }
    },
    new Animal
    {
        Id = 2,
        Name = "Grzes",
        Category = "cat",
        Weight = 4,
        FurColor = "black",
        Visits = new List<Visit>
        {
            new Visit { Id = 1, Date = DateTime.Parse("2025-04-16 11:00:00", CultureInfo.InvariantCulture), Description = "Vaccination", Price = 100 }
        }
    }
};

app.MapGet("/animals", () => animals);

app.MapGet("/animals/{id:int}", (int id) =>
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    return animal is not null ? Results.Ok(animal) : Results.NotFound();
});

app.MapPost("/animals", (Animal animal) =>
{
    animal.Id = animals.Any() ? animals.Max(a => a.Id) + 1 : 1;
    animal.Visits = new List<Visit>();
    animals.Add(animal);
    return Results.Created($"/animals/{animal.Id}", animal);
});

app.MapPut("/animals/{id:int}", (int id, Animal updatedAnimal) =>
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    if (animal is null)
        return Results.NotFound();
    animal.Name = updatedAnimal.Name;
    animal.Category = updatedAnimal.Category;
    animal.Weight = updatedAnimal.Weight;
    animal.FurColor = updatedAnimal.FurColor;
    return Results.Ok(animal);
});

app.MapDelete("/animals/{id:int}", (int id) =>
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    if (animal is null)
        return Results.NotFound();
    animals.Remove(animal);
    return Results.NoContent();
});

app.MapGet("/animals/{id:int}/visits", (int id) =>
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    if (animal is null)
        return Results.NotFound("Animal not found");
    return Results.Ok(animal.Visits);
});

app.MapPost("/animals/{id:int}/visits", (int id, Visit newVisit) =>
{
    var animal = animals.FirstOrDefault(a => a.Id == id);
    if (animal is null)
        return Results.NotFound("Animal not found");
    newVisit.Id = animal.Visits.Any() ? animal.Visits.Max(v => v.Id) + 1 : 1;
    animal.Visits.Add(newVisit);
    return Results.Created($"/animals/{id}/visits/{newVisit.Id}", newVisit);
});

app.Run();
