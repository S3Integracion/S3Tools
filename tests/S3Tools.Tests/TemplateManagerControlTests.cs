using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using Xunit;

namespace S3Tools.Tests
{
    public class TemplateManagerControlTests : IDisposable
    {
        private const string ValidSitemap =
            "{\"_id\":\"x\",\"startUrl\":[],\"selectors\":[" +
            "{\"id\":\"title\",\"type\":\"SelectorText\",\"parentSelectors\":[\"_root\"],\"selector\":\"h1\",\"multiple\":false}]}";

        private readonly string _dir;

        public TemplateManagerControlTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "S3ToolsTests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_dir))
            {
                Directory.Delete(_dir, true);
            }
        }

        [Fact]
        public void ListShowsTemplates_AndFollowsRepositoryChangesUntilDisposed()
        {
            var repository = new SitemapTemplateRepository(Path.Combine(_dir, "sitemap-templates.json"));

            RunSta(() =>
            {
                var control = new TemplateManagerControl(repository);
                var list = Find<ListView>(control);

                Assert.Equal(repository.GetAll().Count, list.Items.Count);
                Assert.Contains(list.Items.Cast<ListViewItem>(), i => i.Text.EndsWith(" · Predeterminada", StringComparison.Ordinal));
                Assert.Equal(repository.GetAll()[0].Id, control.SelectedTemplateId);
                Assert.False(control.HasUnsavedChanges);

                var created = repository.Create(new SitemapTemplateDraft { Name = "Externa", SitemapJson = ValidSitemap });
                Assert.True(created.Ok);
                Assert.Equal(repository.GetAll().Count, list.Items.Count);

                control.Dispose();
                Assert.True(repository.Create(new SitemapTemplateDraft { Name = "Tras cerrar", SitemapJson = ValidSitemap }).Ok);
            });
        }

        private static T Find<T>(Control parent) where T : Control
        {
            foreach (Control child in parent.Controls)
            {
                if (child is T match)
                {
                    return match;
                }

                var nested = Find<T>(child);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        /// <summary>Los controles WinForms requieren un hilo STA.</summary>
        private static void RunSta(Action action)
        {
            ExceptionDispatchInfo failure = null;
            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    failure = ExceptionDispatchInfo.Capture(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            failure?.Throw();
        }
    }
}
