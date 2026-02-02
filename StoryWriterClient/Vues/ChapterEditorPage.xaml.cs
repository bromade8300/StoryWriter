using Newtonsoft.Json;
using StoryWriter.Classes;
using System.Text;
using System.Text.RegularExpressions;

namespace StoryWriterClient.Vues
{
    public partial class ChapterEditorPage : ContentPage
    {
        private const string ApiBaseUrl = "https://localhost:7056";
        private Book _book;
        private Chapter _chapter;
        private bool _isNew;

        public ChapterEditorPage(Book book, Chapter chapter, bool isNew)
        {
            InitializeComponent();
            _book = book;
            _chapter = chapter;
            _isNew = isNew;

            Title = isNew ? "Nouveau chapitre" : "Modifier le chapitre";

            ChapterNumberEntry.Text = chapter.Number.ToString();
            ChapterNameEntry.Text = chapter.Name;

            LoadEditor();
        }

        private void LoadEditor()
        {
            // Decode unicode escapes like \u003C to actual characters
            string content = _chapter.Content ?? "";
            if (content.Contains("\\u"))
            {
                content = Regex.Unescape(content);
            }

            // Escape for JavaScript string
            string escapedContent = content.Replace("\\", "\\\\")
                                           .Replace("'", "\\'")
                                           .Replace("\"", "\\\"")
                                           .Replace("\n", "\\n")
                                           .Replace("\r", "\\r");

            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link href='https://cdn.quilljs.com/1.3.6/quill.snow.css' rel='stylesheet'>
    <style>
        html, body {{
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background-color: #FFFFFF !important;
            color: #000000 !important;
            color-scheme: light;
        }}
        #editor {{
            height: calc(100vh - 50px);
            font-size: 16px;
            background-color: #FFFFFF !important;
        }}
        .ql-container {{
            font-size: 16px;
            background-color: #FFFFFF !important;
            border: none !important;
        }}
        .ql-toolbar {{ display: none; }}
        .ql-editor {{
            padding: 15px;
            background-color: #FFFFFF !important;
            color: #000000 !important;
        }}
        .ql-editor.ql-blank::before {{
            color: #999999 !important;
        }}
    </style>
</head>
<body>
    <div id='editor'></div>
    <script src='https://cdn.quilljs.com/1.3.6/quill.min.js'></script>
    <script>
        var quill = new Quill('#editor', {{
            theme: 'snow',
            modules: {{
                toolbar: false
            }},
            placeholder: 'Écrivez votre chapitre ici...'
        }});

        // Load initial content
        quill.root.innerHTML = '{escapedContent}';

        function getContent() {{
            return quill.root.innerHTML;
        }}

        function formatBold() {{
            var range = quill.getSelection();
            if (range) {{
                var format = quill.getFormat(range);
                quill.format('bold', !format.bold);
            }}
        }}

        function formatItalic() {{
            var range = quill.getSelection();
            if (range) {{
                var format = quill.getFormat(range);
                quill.format('italic', !format.italic);
            }}
        }}

        function formatUnderline() {{
            var range = quill.getSelection();
            if (range) {{
                var format = quill.getFormat(range);
                quill.format('underline', !format.underline);
            }}
        }}

        function formatH1() {{
            var range = quill.getSelection();
            if (range) {{
                var format = quill.getFormat(range);
                quill.format('header', format.header === 1 ? false : 1);
            }}
        }}

        function formatH2() {{
            var range = quill.getSelection();
            if (range) {{
                var format = quill.getFormat(range);
                quill.format('header', format.header === 2 ? false : 2);
            }}
        }}
    </script>
</body>
</html>";

            RichTextEditor.Source = new HtmlWebViewSource { Html = html };
        }

        private async void OnBoldClicked(object sender, EventArgs e)
        {
            await RichTextEditor.EvaluateJavaScriptAsync("formatBold()");
        }

        private async void OnItalicClicked(object sender, EventArgs e)
        {
            await RichTextEditor.EvaluateJavaScriptAsync("formatItalic()");
        }

        private async void OnUnderlineClicked(object sender, EventArgs e)
        {
            await RichTextEditor.EvaluateJavaScriptAsync("formatUnderline()");
        }

        private async void OnH1Clicked(object sender, EventArgs e)
        {
            await RichTextEditor.EvaluateJavaScriptAsync("formatH1()");
        }

        private async void OnH2Clicked(object sender, EventArgs e)
        {
            await RichTextEditor.EvaluateJavaScriptAsync("formatH2()");
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ChapterNameEntry.Text))
            {
                await DisplayAlert("Erreur", "Le titre du chapitre est requis", "OK");
                return;
            }

            if (!int.TryParse(ChapterNumberEntry.Text, out int chapterNumber))
            {
                await DisplayAlert("Erreur", "Le numéro du chapitre doit être un nombre", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsRunning = true;
                LoadingIndicator.IsVisible = true;
                SaveButton.IsEnabled = false;

                // Get content from editor
                string content = await RichTextEditor.EvaluateJavaScriptAsync("getContent()");

                // Clean up the content (remove quotes if present from JSON)
                if (content != null && content.StartsWith("\"") && content.EndsWith("\""))
                {
                    content = content.Substring(1, content.Length - 2);
                }

                // Decode JSON escaped characters
                if (!string.IsNullOrEmpty(content))
                {
                    // Decode unicode escapes and other JSON escapes
                    content = Regex.Unescape(content);
                }

                _chapter.Name = ChapterNameEntry.Text;
                _chapter.Number = chapterNumber;
                _chapter.Content = content ?? "";

                using HttpClient client = new HttpClient();
                string json = JsonConvert.SerializeObject(_chapter);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (_isNew)
                {
                    response = await client.PostAsync($"{ApiBaseUrl}/Book/{_book.Id}/chapters", httpContent);
                }
                else
                {
                    response = await client.PutAsync($"{ApiBaseUrl}/Book/{_book.Id}/chapters/{_chapter.Id}", httpContent);
                }

                if (response.IsSuccessStatusCode)
                {
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erreur", "Impossible de sauvegarder le chapitre", "OK");
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
