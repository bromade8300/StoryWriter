using Newtonsoft.Json;
using StoryWriter.Classes;
using StoryWriterClient.Vues;
using System.Net.Http.Json;

namespace StoryWriterClient
{
    public partial class MainPage : ContentPage
    {
        private const string ApiBaseUrl = "https://localhost:7056";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadListBooks();
        }

        private async void OnCreateBookClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateBook());
        }

        private async void OnBookSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Book selectedBook)
            {
                BooksCollectionView.SelectedItem = null;
                await Navigation.PushAsync(new BookDetailPage(selectedBook));
            }
        }

        private async Task<List<Book>> GetListBook()
        {
            using HttpClient client = new HttpClient();
            return await client.GetFromJsonAsync<List<Book>>($"{ApiBaseUrl}/Book") ?? new List<Book>();
        }

        private async void LoadListBooks()
        {
            try
            {
                LoadingIndicator.IsRunning = true;
                LoadingIndicator.IsVisible = true;

                var books = await GetListBook();

                // Ensure lists are initialized for binding
                foreach (var book in books)
                {
                    book.Chapters ??= new List<Chapter>();
                    book.Characters ??= new List<Character>();
                }

                BooksCollectionView.ItemsSource = books;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible de récupérer les livres: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }
    }
}
