using System.Linq;
using System.Text;

namespace ParentChildBuilderPatternDemo
{
    class Chapter
    {        
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? OpeningQuote { get; set; }

        public override string ToString()
        {
            return $"{nameof(Chapter)}:\n" +
                $"{nameof(Title)}: {Title}\n" +
                $"{nameof(Content)}: {Content}\n" +
                $"{nameof(OpeningQuote)}: {OpeningQuote}\n";
        }
    }
    class Book
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public IList<Chapter>? Chapters { get; set; }
        private string FormattingBook()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Book Cover:")
                .AppendLine($"\t{nameof(Title)}: {Title}")
                .AppendLine($"\t{nameof(Author)}: {Author}")
                .AppendLine($"\t{nameof(Publisher)}: {Publisher}")
                .AppendLine();

            sb.AppendLine(
                string.Join('\n', Chapters.Select(chapter => chapter.ToString()).ToList()));

            return sb.ToString();
        }

        public override string ToString()
        {
            return FormattingBook();    
        }
    }

    interface IBookBuilder
    {
        BookCoverBuilder AddBookCover();
        BookChapterBuilder AddChapter();
        Book Build();
    }

    class BookCoverBuilder
    {
        private readonly BookBuilder _parentBookBuilder;
        private readonly Book _book;

        public BookCoverBuilder(BookBuilder parentBookBuilder, Book book)
        {
            _parentBookBuilder = parentBookBuilder;
            _book = book;
        }

        public BookCoverBuilder WithTitle(string title)
        {
            _book.Title = title;
            return this;
        }

        public BookCoverBuilder WithAuthor(string author)
        {
            _book.Author = author;
            return this;
        }

        public BookCoverBuilder WithPublisher(string publisher)
        {
            _book.Publisher = publisher;
            return this;
        }

        public BookBuilder BuildBookCoverComplete()
        {
            return _parentBookBuilder;
        }
    }

    class BookChapterBuilder
    {
        private readonly BookBuilder _parentBookBuilder;
        private readonly Book _book;
        private readonly Chapter _chapter = new Chapter();

        public BookChapterBuilder(BookBuilder parentBookBuilder, Book book)
        {
            _parentBookBuilder = parentBookBuilder;
            _book = book;
        }

        public BookChapterBuilder WithTitle(string title)
        {
            _chapter.Title = title;
            return this;
        }

        public BookChapterBuilder WithContent(string content)
        {
            _chapter.Content = content;
            return this;
        }

        public BookChapterBuilder WithOpeningQuote(string openingQuote)
        {
            _chapter.OpeningQuote = openingQuote;
            return this;
        }

        // not book cover because the book cover must be added before adding chapters
        public BookChapterBuilder AddNextChapter()
        {
           _book.Chapters.Add(_chapter);
           return new BookChapterBuilder(_parentBookBuilder, _book);
        }        

        public BookBuilder BuildChapterComplete()
        {
            _book.Chapters.Add(_chapter);
            return _parentBookBuilder;
        }        
    }

    class BookBuilder
    {
        private readonly Book _book = new Book();

        public BookBuilder()
        {
            _book.Chapters = new List<Chapter>();
        }

        public BookChapterBuilder AddChapter()
        {
            return new BookChapterBuilder(this, _book);
        }

        public BookCoverBuilder AddBookCover()
        {
            return new BookCoverBuilder(this, _book);
        }

        public Book BuildComplete()
        {
            return _book;
        }
    }

    internal class Program
    {        
        static void Main(string[] args)
        {
            Book book = new BookBuilder()
                .AddBookCover()
                    .WithPublisher("Publisher")
                    .WithTitle("Cover Title")
                    .BuildBookCoverComplete()
                .AddChapter()
                    .WithTitle("Chapter Title 1")
                    .WithContent("Long Content")
                    .WithOpeningQuote("Opening Quote 1")
                .AddNextChapter()
                    .WithTitle("Chapter Title 2")
                    .WithContent("Long Content")
                    .WithOpeningQuote("Opening Quote 2")
                .AddNextChapter()
                    .WithTitle("Chapter Title 3")
                    .WithContent("Long Content")
                    .WithOpeningQuote("Opening Quote 3")
                    .BuildChapterComplete()
                .BuildComplete();

            Console.WriteLine(book);
        }
    }
}