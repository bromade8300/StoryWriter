namespace StoryWriterClient.Vues;

using StoryWriter.Classes;
using System.Text;
using Newtonsoft.Json;
using MongoDB.Bson;

public partial class CreateBook : ContentPage
{
    private const string ApiBaseUrl = "https://localhost:7056";
    private Book? _existingBook;
    private bool _isEditMode;

    public CreateBook()
    {
        InitializeComponent();
        _isEditMode = false;
    }

    public CreateBook(Book bookToEdit)
    {
        InitializeComponent();
        _existingBook = bookToEdit;
        _isEditMode = true;

        Title = "Modifier le livre";
        SaveButton.Text = "Enregistrer";

        // Load existing data
        TitleEntry.Text = bookToEdit.Title;
        AuthorEntry.Text = bookToEdit.Author;
        SynopsisEditor.Text = bookToEdit.Synopsis;
        CoverUrlEntry.Text = bookToEdit.CoverImageUrl;

        if (!string.IsNullOrEmpty(bookToEdit.CoverImageUrl))
        {
            CoverPreview.Source = bookToEdit.CoverImageUrl;
            CoverPreview.IsVisible = true;
            CoverIcon.IsVisible = false;
        }
    }

    private void OnCoverUrlChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue) && Uri.IsWellFormedUriString(e.NewTextValue, UriKind.Absolute))
        {
            CoverPreview.Source = e.NewTextValue;
            CoverPreview.IsVisible = true;
            CoverIcon.IsVisible = false;
        }
        else
        {
            CoverPreview.IsVisible = false;
            CoverIcon.IsVisible = true;
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddBookClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(AuthorEntry.Text))
        {
            await DisplayAlert("Erreur", "Le titre et l'auteur sont requis", "OK");
            return;
        }

        try
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;
            SaveButton.IsEnabled = false;

            Book book;
            if (_isEditMode && _existingBook != null)
            {
                book = _existingBook;
                book.Title = TitleEntry.Text.Trim();
                book.Author = AuthorEntry.Text.Trim();
                book.Synopsis = SynopsisEditor.Text;
                book.CoverImageUrl = CoverUrlEntry.Text;
            }
            else
            {
                book = new Book
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Title = TitleEntry.Text.Trim(),
                    Author = AuthorEntry.Text.Trim(),
                    Synopsis = SynopsisEditor.Text,
                    CoverImageUrl = CoverUrlEntry.Text,
                    CreatedAt = DateTime.UtcNow,
                    Chapters = new List<Chapter>(),
                    Characters = new List<Character>()
                };
            }

            string json = JsonConvert.SerializeObject(book);

            using HttpClient client = new HttpClient();
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            if (_isEditMode)
            {
                response = await client.PutAsync($"{ApiBaseUrl}/Book/{book.Id}", content);
            }
            else
            {
                response = await client.PostAsync($"{ApiBaseUrl}/Book", content);
            }

            if (response.IsSuccessStatusCode)
            {
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erreur", "Impossible de sauvegarder le livre", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur s'est produite: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            SaveButton.IsEnabled = true;
        }
    }
}
