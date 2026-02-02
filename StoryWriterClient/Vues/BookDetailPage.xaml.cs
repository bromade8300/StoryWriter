using MongoDB.Bson;
using Newtonsoft.Json;
using StoryWriter.Classes;
using System.Net.Http.Json;
using System.Text;
using System.Windows.Input;

namespace StoryWriterClient.Vues
{
    public partial class BookDetailPage : ContentPage
    {
        private const string ApiBaseUrl = "https://localhost:7056";
        private Book _book;

        public ICommand DeleteChapterCommand { get; }
        public ICommand DeleteCharacterCommand { get; }

        public BookDetailPage(Book book)
        {
            InitializeComponent();
            _book = book;
            BindingContext = this;

            DeleteChapterCommand = new Command<Chapter>(async (chapter) => await DeleteChapter(chapter));
            DeleteCharacterCommand = new Command<Character>(async (character) => await DeleteCharacter(character));

            LoadBookData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshBookData();
        }

        private void LoadBookData()
        {
            TitleLabel.Text = _book.Title;
            AuthorLabel.Text = _book.Author;
            SynopsisLabel.Text = _book.Synopsis ?? "Pas de synopsis";

            if (!string.IsNullOrEmpty(_book.CoverImageUrl))
            {
                CoverImage.Source = _book.CoverImageUrl;
            }

            UpdateCounts();
            LoadChapters();
            LoadCharacters();
        }

        private async void RefreshBookData()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var refreshedBook = await client.GetFromJsonAsync<Book>($"{ApiBaseUrl}/Book/{_book.Id}");
                if (refreshedBook != null)
                {
                    _book = refreshedBook;
                    _book.Chapters ??= new List<Chapter>();
                    _book.Characters ??= new List<Character>();
                    LoadBookData();
                }
            }
            catch (Exception ex)
            {
                // Silently fail on refresh
            }
        }

        private void UpdateCounts()
        {
            var chapterCount = _book.Chapters?.Count ?? 0;
            var characterCount = _book.Characters?.Count ?? 0;
            ChapterCountLabel.Text = $"{chapterCount} chapitre{(chapterCount != 1 ? "s" : "")}";
            CharacterCountLabel.Text = $"{characterCount} personnage{(characterCount != 1 ? "s" : "")}";
        }

        private void LoadChapters()
        {
            var sortedChapters = _book.Chapters?.OrderBy(c => c.Number).ToList() ?? new List<Chapter>();
            ChaptersCollectionView.ItemsSource = sortedChapters;
        }

        private void LoadCharacters()
        {
            CharactersCollectionView.ItemsSource = _book.Characters ?? new List<Character>();
        }

        // Tab switching
        private void OnChaptersTabClicked(object sender, EventArgs e)
        {
            ChaptersTabButton.BackgroundColor = Color.FromArgb("#512BD4");
            ChaptersTabButton.TextColor = Colors.White;
            ChaptersTabButton.FontAttributes = FontAttributes.Bold;

            CharactersTabButton.BackgroundColor = Colors.White;
            CharactersTabButton.TextColor = Color.FromArgb("#512BD4");
            CharactersTabButton.FontAttributes = FontAttributes.None;

            ChaptersSection.IsVisible = true;
            CharactersSection.IsVisible = false;
        }

        private void OnCharactersTabClicked(object sender, EventArgs e)
        {
            CharactersTabButton.BackgroundColor = Color.FromArgb("#512BD4");
            CharactersTabButton.TextColor = Colors.White;
            CharactersTabButton.FontAttributes = FontAttributes.Bold;

            ChaptersTabButton.BackgroundColor = Colors.White;
            ChaptersTabButton.TextColor = Color.FromArgb("#512BD4");
            ChaptersTabButton.FontAttributes = FontAttributes.None;

            ChaptersSection.IsVisible = false;
            CharactersSection.IsVisible = true;
        }

        // Navigation
        private async void OnEditBookClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateBook(_book));
        }

        private async void OnDeleteBookClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Supprimer", $"Voulez-vous vraiment supprimer \"{_book.Title}\" ?", "Oui", "Non");
            if (confirm)
            {
                try
                {
                    using HttpClient client = new HttpClient();
                    var response = await client.DeleteAsync($"{ApiBaseUrl}/Book/{_book.Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        await DisplayAlert("Erreur", "Impossible de supprimer le livre", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", ex.Message, "OK");
                }
            }
        }

        private async void OnAddChapterClicked(object sender, EventArgs e)
        {
            int nextNumber = (_book.Chapters?.Count ?? 0) + 1;
            var newChapter = new Chapter
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = $"Chapitre {nextNumber}",
                Number = nextNumber,
                Content = ""
            };
            await Navigation.PushAsync(new ChapterEditorPage(_book, newChapter, isNew: true));
        }

        private async void OnChapterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Chapter selectedChapter)
            {
                ChaptersCollectionView.SelectedItem = null;
                await Navigation.PushAsync(new ChapterEditorPage(_book, selectedChapter, isNew: false));
            }
        }

        private async Task DeleteChapter(Chapter chapter)
        {
            bool confirm = await DisplayAlert("Supprimer", $"Supprimer le chapitre \"{chapter.Name}\" ?", "Oui", "Non");
            if (confirm)
            {
                try
                {
                    using HttpClient client = new HttpClient();
                    var response = await client.DeleteAsync($"{ApiBaseUrl}/Book/{_book.Id}/chapters/{chapter.Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        _book.Chapters?.Remove(chapter);
                        UpdateCounts();
                        LoadChapters();
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", ex.Message, "OK");
                }
            }
        }

        private async void OnAddCharacterClicked(object sender, EventArgs e)
        {
            var newCharacter = new Character
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = ""
            };
            await Navigation.PushAsync(new CharacterEditorPage(_book, newCharacter, isNew: true));
        }

        private async void OnCharacterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Character selectedCharacter)
            {
                CharactersCollectionView.SelectedItem = null;
                await Navigation.PushAsync(new CharacterEditorPage(_book, selectedCharacter, isNew: false));
            }
        }

        private async Task DeleteCharacter(Character character)
        {
            bool confirm = await DisplayAlert("Supprimer", $"Supprimer le personnage \"{character.Name}\" ?", "Oui", "Non");
            if (confirm)
            {
                try
                {
                    using HttpClient client = new HttpClient();
                    var response = await client.DeleteAsync($"{ApiBaseUrl}/Book/{_book.Id}/characters/{character.Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        _book.Characters?.Remove(character);
                        UpdateCounts();
                        LoadCharacters();
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erreur", ex.Message, "OK");
                }
            }
        }
    }
}
