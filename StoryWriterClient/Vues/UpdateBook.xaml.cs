namespace StoryWriterClient.Vues;

public partial class UpdateBook : ContentPage
{
	public UpdateBook()
	{
		InitializeComponent();
        //LoadEditor();
        LoadHtmlFile();
    }


    private async void LoadHtmlFile()
    {
        // Ouvrir le fichier HTML stocké dans les ressources de l'application
        using var stream = await FileSystem.OpenAppPackageFileAsync("Editor.html");
        using var reader = new StreamReader(stream);
        string htmlContent = await reader.ReadToEndAsync();

        // Charger le HTML dans le WebView
        RichTextEditor.Source = new HtmlWebViewSource
        {
            Html = htmlContent
        };
    }

    private void LoadEditor()
    {
        string html = @"
            <html>
            <head>
                <link href='https://cdn.quilljs.com/1.3.6/quill.snow.css' rel='stylesheet'>
            </head>
            <body>
                <div id='editor' style='height: 300px;'></div>
                <script src='https://cdn.quilljs.com/1.3.6/quill.min.js'></script>
                <script>
                    var quill = new Quill('#editor', { theme: 'snow' });

                    function getContent() {
                        return quill.root.innerHTML;
                    }
                </script>
            </body>
            </html>";

        RichTextEditor.Source = new HtmlWebViewSource { Html = html };
    }

    public async void GetContentClicked(object sender, EventArgs e)
    {
        string content = await RichTextEditor.EvaluateJavaScriptAsync("getContent()");
        ContentLabel.Text = content;
    }
}

