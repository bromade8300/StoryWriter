using Microsoft.Maui.Controls.Shapes;
using MongoDB.Bson;
using Newtonsoft.Json;
using StoryWriter.Classes;
using System.Text;

namespace StoryWriterClient.Vues
{
    public partial class CharacterEditorPage : ContentPage
    {
        private const string ApiBaseUrl = "https://localhost:7056";
        private Book _book;
        private Character _character;
        private bool _isNew;

        public CharacterEditorPage(Book book, Character character, bool isNew)
        {
            InitializeComponent();
            _book = book;
            _character = character;
            _isNew = isNew;

            Title = isNew ? "Nouveau personnage" : "Modifier le personnage";

            LoadCharacterData();
        }

        private void LoadCharacterData()
        {
            NameEntry.Text = _character.Name;
            DescriptionEditor.Text = _character.Description;
            NotesEditor.Text = _character.Notes;
            ImageUrlEntry.Text = _character.ImageUrl;

            // Set role picker
            if (!string.IsNullOrEmpty(_character.Role))
            {
                int index = ((IList<string>)RolePicker.ItemsSource).IndexOf(_character.Role);
                if (index >= 0)
                {
                    RolePicker.SelectedIndex = index;
                }
            }

            // Set traits
            if (_character.Traits != null && _character.Traits.Count > 0)
            {
                TraitsEntry.Text = string.Join(", ", _character.Traits);
                UpdateTraitsDisplay();
            }

            // Set image if available
            if (!string.IsNullOrEmpty(_character.ImageUrl))
            {
                CharacterImage.Source = _character.ImageUrl;
                CharacterImage.IsVisible = true;
                AvatarIcon.IsVisible = false;
            }
        }

        private void UpdateTraitsDisplay()
        {
            TraitsContainer.Children.Clear();

            if (string.IsNullOrWhiteSpace(TraitsEntry.Text))
                return;

            var traits = TraitsEntry.Text.Split(',')
                                         .Select(t => t.Trim())
                                         .Where(t => !string.IsNullOrEmpty(t));

            foreach (var trait in traits)
            {
                var traitBorder = new Border
                {
                    StrokeShape = new RoundRectangle { CornerRadius = 15 },
                    BackgroundColor = Color.FromArgb("#E8E0F0"),
                    Padding = new Thickness(12, 6),
                    Margin = new Thickness(0, 0, 8, 8)
                };

                traitBorder.Content = new Label
                {
                    Text = trait,
                    FontSize = 12,
                    TextColor = Color.FromArgb("#512BD4")
                };

                TraitsContainer.Children.Add(traitBorder);
            }
        }

        private void OnImageUrlChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewTextValue) && Uri.IsWellFormedUriString(e.NewTextValue, UriKind.Absolute))
            {
                CharacterImage.Source = e.NewTextValue;
                CharacterImage.IsVisible = true;
                AvatarIcon.IsVisible = false;
            }
            else
            {
                CharacterImage.IsVisible = false;
                AvatarIcon.IsVisible = true;
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                await DisplayAlert("Erreur", "Le nom du personnage est requis", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsRunning = true;
                LoadingIndicator.IsVisible = true;
                SaveButton.IsEnabled = false;

                _character.Name = NameEntry.Text.Trim();
                _character.Role = RolePicker.SelectedItem?.ToString();
                _character.Description = DescriptionEditor.Text;
                _character.Notes = NotesEditor.Text;
                _character.ImageUrl = ImageUrlEntry.Text;

                // Parse traits
                if (!string.IsNullOrWhiteSpace(TraitsEntry.Text))
                {
                    _character.Traits = TraitsEntry.Text
                        .Split(',')
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToList();
                }
                else
                {
                    _character.Traits = null;
                }

                using HttpClient client = new HttpClient();
                string json = JsonConvert.SerializeObject(_character);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (_isNew)
                {
                    response = await client.PostAsync($"{ApiBaseUrl}/Book/{_book.Id}/characters", httpContent);
                }
                else
                {
                    response = await client.PutAsync($"{ApiBaseUrl}/Book/{_book.Id}/characters/{_character.Id}", httpContent);
                }

                if (response.IsSuccessStatusCode)
                {
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erreur", "Impossible de sauvegarder le personnage", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                SaveButton.IsEnabled = true;
            }
        }
    }
}
