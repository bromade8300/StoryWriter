namespace StoryWriterClient.Vues;
using StoryWriter.Classes;
using StoryWriter.BDD;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using MongoDB.Bson;

public partial class CreateBook : ContentPage
{
	public CreateBook()
	{
		InitializeComponent();
	}

    private async void OnAddBookClicked(object sender, EventArgs e)
    {

        if(TitleEntry.Text == null || AuthorEntry.Text == null)
        {
            await DisplayAlert("Error", "Please fill in all the fields.", "OK");
            return;
        } else
        {

            Book book = new Book
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = TitleEntry.Text,
                Author = AuthorEntry.Text,
            };

            string json = JsonConvert.SerializeObject(book);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://localhost:7056/Book", content);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "Book added successfully!", "OK");
                        await Navigation.PushAsync(new MainPage());
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to add the book.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }

        }

    }

}