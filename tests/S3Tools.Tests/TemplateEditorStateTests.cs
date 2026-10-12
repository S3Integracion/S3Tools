using System;
using Xunit;

namespace S3Tools.Tests
{
    public class TemplateEditorStateTests
    {
        private const string Json = "{\r\n  \"_id\": \"x\",\r\n  \"selectors\": []\r\n}";

        private static SitemapTemplate Template(string name = "Mi plantilla", string description = "desc", params string[] stores)
        {
            var now = new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc);
            return new SitemapTemplate("abc123", name, description, null, stores, now, now, Json);
        }

        private static TemplateEditorState Loaded(SitemapTemplate template)
        {
            var state = new TemplateEditorState();
            state.Load(template);
            return state;
        }

        [Fact]
        public void Load_CopiesFieldsAndIsNotDirty()
        {
            var state = Loaded(Template("Mi plantilla", "desc", "Altinor", "ProductosTX"));

            Assert.False(state.IsDirty);
            Assert.False(state.IsNew);
            Assert.Equal("abc123", state.EditingId);
            Assert.Equal("Mi plantilla", state.Name);
            Assert.Equal("desc", state.Description);
            Assert.Equal(new[] { "Altinor", "ProductosTX" }, state.Stores);
            Assert.Equal(Json, state.SitemapJson);
        }

        [Fact]
        public void ChangingNameMakesDirty_AndRestoringOriginalClearsIt()
        {
            var state = Loaded(Template("Mi plantilla"));

            state.Name = "Otra";
            Assert.True(state.IsDirty);

            state.Name = "Mi plantilla";
            Assert.False(state.IsDirty);
        }

        [Fact]
        public void NameAndDescription_ComparedTrimmed_NullDescriptionEqualsEmpty()
        {
            var state = Loaded(Template("Mi plantilla", string.Empty));

            state.Name = "  Mi plantilla ";
            state.Description = null;
            Assert.False(state.IsDirty);

            state.Description = "  ";
            Assert.False(state.IsDirty);
        }

        [Fact]
        public void ChangingJson_MakesDirty_ExactTextComparison()
        {
            var state = Loaded(Template());

            state.SitemapJson = Json + " ";
            Assert.True(state.IsDirty);

            state.SitemapJson = Json;
            Assert.False(state.IsDirty);
        }

        [Fact]
        public void Stores_SameSetDifferentOrderOrCase_IsNotDirty()
        {
            var state = Loaded(Template("Mi plantilla", "desc", "Altinor", "ProductosTX"));

            state.Stores = new[] { "productostx", "Altinor" };
            Assert.False(state.IsDirty);

            state.Stores = new[] { "Altinor" };
            Assert.True(state.IsDirty);
        }

        [Fact]
        public void Stores_SetterCopiesTheList()
        {
            var state = Loaded(Template("Mi plantilla", "desc", "Altinor"));
            var stores = new[] { "Altinor" };
            state.Stores = stores;

            stores[0] = "BBvsBB2";

            Assert.Equal(new[] { "Altinor" }, state.Stores);
            Assert.False(state.IsDirty);
        }

        [Fact]
        public void LoadNew_IsNewAndNotDirtyUntilEdited()
        {
            var state = Loaded(Template());

            state.LoadNew();

            Assert.True(state.IsNew);
            Assert.Null(state.EditingId);
            Assert.Equal(string.Empty, state.Name);
            Assert.Equal(string.Empty, state.Description);
            Assert.Empty(state.Stores);
            Assert.False(state.IsDirty);

            state.Name = "Nueva";
            Assert.True(state.IsDirty);
        }

        [Fact]
        public void LoadNew_StarterJsonIsAValidSitemap()
        {
            var state = new TemplateEditorState();
            state.LoadNew();

            var validation = SitemapTemplateValidator.Validate(state.SitemapJson);

            Assert.True(validation.IsValid);
            Assert.Single(validation.Selectors);
        }

        [Fact]
        public void Load_NormalizesLineEndingsToCrLf_AndIsNotDirty()
        {
            var now = DateTime.UtcNow;
            var template = new SitemapTemplate("lf", "LF", string.Empty, null, null, now, now, "{\n  \"a\": 1\r\n}\n");

            var state = Loaded(template);

            Assert.Equal("{\r\n  \"a\": 1\r\n}\r\n", state.SitemapJson);
            Assert.False(state.IsDirty);
        }

        [Fact]
        public void Load_AfterEdits_ResetsDirty()
        {
            var state = Loaded(Template());
            state.Name = "Cambiado";

            state.Load(Template("Guardada"));

            Assert.False(state.IsDirty);
            Assert.Equal("Guardada", state.Name);
        }

        [Fact]
        public void ToDraft_TrimsNameAndDescription_AndCopiesFields()
        {
            var state = Loaded(Template("Mi plantilla", "desc", "Altinor"));
            state.Name = "  Nombre nuevo  ";
            state.Description = " algo ";

            var draft = state.ToDraft();

            Assert.Equal("Nombre nuevo", draft.Name);
            Assert.Equal("algo", draft.Description);
            Assert.Equal(new[] { "Altinor" }, draft.Stores);
            Assert.Equal(Json, draft.SitemapJson);
        }

        [Fact]
        public void ToDraft_NullDescription_BecomesEmpty()
        {
            var state = new TemplateEditorState();
            state.LoadNew();
            state.Description = null;

            Assert.Equal(string.Empty, state.ToDraft().Description);
        }
    }
}
