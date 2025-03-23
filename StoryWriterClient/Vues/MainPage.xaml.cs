using MongoDB.Bson;
using Newtonsoft.Json;
using StoryWriter.BDD;
using StoryWriter.Classes;
using StoryWriterClient.Vues;
using System.Net.Http.Json;
using System.Text.Json;

namespace StoryWriterClient
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            LoadListBooks();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateBook());
        }

        private async Task<List<Book>> getListBook()
        {
            HttpClient client = new HttpClient();
            return await client.GetFromJsonAsync<List<Book>>("https://localhost:7056/Book") ?? new List<Book>();
        }

        private async void LoadListBooks()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        BooksListView.ItemsSource = await getListBook();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible de récupérer les livres: {ex.Message}", "OK");
            }
        }

    }

}
