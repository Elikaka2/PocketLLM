using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace PocketLLM.Services
{
    public class MarkdownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string markdown = value as string;
            if (string.IsNullOrWhiteSpace(markdown))
                return new FlowDocument();

            markdown = string.Join("\n", markdown.Split('\n').Select(l => l.TrimEnd()));

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);

            var flowDoc = new FlowDocument
            {
                FontFamily = new System.Windows.Media.FontFamily("Noto Sans"),
                FontSize = 18,
                Foreground = System.Windows.Media.Brushes.White,
                TextAlignment = System.Windows.TextAlignment.Left,
                PagePadding = new System.Windows.Thickness(0)
            };

            foreach (var block in document)
            {
                if (block is ParagraphBlock para)
                {
                    var paragraph = new Paragraph
                    {
                        Margin = new System.Windows.Thickness(0),
                        TextAlignment = System.Windows.TextAlignment.Left
                    };
                    AddInline(paragraph.Inlines, para.Inline);
                    flowDoc.Blocks.Add(paragraph);
                }
                else if (block is ListBlock listBlock)
                {
                    var list = new List
                    {
                        Margin = new System.Windows.Thickness(0)
                    };

                    foreach (ListItemBlock item in listBlock)
                    {
                        var listItem = new ListItem();
                        foreach (var subBlock in item)
                        {
                            if (subBlock is ParagraphBlock subPara)
                            {
                                var paragraph = new Paragraph
                                {
                                    Margin = new System.Windows.Thickness(0),
                                    TextAlignment = System.Windows.TextAlignment.Left
                                };
                                AddInline(paragraph.Inlines, subPara.Inline);
                                listItem.Blocks.Add(paragraph);
                            }
                        }
                        list.ListItems.Add(listItem);
                    }
                    flowDoc.Blocks.Add(list);
                }
                else if (block is FencedCodeBlock codeBlock)
                {
                    var paragraph = new Paragraph
                    {
                        Margin = new System.Windows.Thickness(2, 2, 2, 2),
                        TextAlignment = System.Windows.TextAlignment.Left
                    };

                    string codeText = "";
                    if (codeBlock.Lines.Lines != null && codeBlock.Lines.Lines != null)
                    {
                        codeText = string.Join("\n",
                            codeBlock.Lines.Lines.Select(l =>
                            {
                                var slice = l.Slice;
                                return slice.Text != null
                                    ? slice.Text.Substring(slice.Start, slice.Length).TrimEnd()
                                    : "";
                            })
                        );
                    }

                    var code = new Run(codeText)
                    {
                        FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                        Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30)),
                        Foreground = System.Windows.Media.Brushes.LightGreen
                    };

                    paragraph.Inlines.Add(code);
                    flowDoc.Blocks.Add(paragraph);
                }
            }

            return flowDoc;
        }

        private void AddInline(InlineCollection inlines, ContainerInline inline)
        {
            if (inline == null) return;

            foreach (var child in inline)
            {
                switch (child)
                {
                    case LiteralInline lit:
                        var text = lit.Content.Text.Substring(lit.Content.Start, lit.Content.Length).TrimEnd();
                        if (!string.IsNullOrEmpty(text))
                            inlines.Add(new Run(text));
                        break;

                    case EmphasisInline em:
                        var span = new Span();
                        AddInline(span.Inlines, em);
                        if (em.DelimiterCount == 2)
                            span.FontWeight = FontWeights.Bold;
                        else
                            span.FontStyle = FontStyles.Italic;
                        inlines.Add(span);
                        break;

                    case LineBreakInline _:
                        inlines.Add(new LineBreak());
                        break;

                    case LinkInline link:
                        var hyperlink = new Hyperlink(new Run(link.Title ?? link.Url))
                        {
                            NavigateUri = new Uri(link.Url)
                        };
                        hyperlink.RequestNavigate += (s, e) =>
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                        };
                        inlines.Add(hyperlink);
                        break;

                    case CodeInline codeInline:
                        string codeStr = codeInline.Content?.Trim() ?? "";
                        if (!string.IsNullOrEmpty(codeStr))
                        {
                            inlines.Add(new Run(" "));
                            inlines.Add(new Run(codeStr)
                            {
                                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 50, 50)),
                                Foreground = System.Windows.Media.Brushes.LightGreen
                            });
                            inlines.Add(new Run(" "));
                        }
                        break;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
