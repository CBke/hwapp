using hwapp;
using Models;
using Extentions;
using System.Collections.Generic;
using System.Linq;
using hwapp.Data;
using System.Collections.Concurrent;

namespace Data
{
    public static class DataMapper
    {
        public static Dictionary<string, string> IdToType = new Dictionary<string, string>()
        {
            //{"PT1",  "Tijdschriftartikel"},
            {"PT2",  "1231"},
            {"PT3",  "1232"},
            //{"PT5",  "Boek als editor"},
            //{"PT6",  "Boek als auteur"},
            {"PT7",  "1248"},
            {"PT9",  "1307"},
            {"PT10", "1299"},
            {"PT11", "1300"},
            //{"PT12", "Letter to the Editor"},
            {"PT13", "1301"},
            {"PT23", "1305"},
            {"PT59", "Noot"},
            {"PT61", "1306"},
            {"PT113","1240"},
            {"PT114","1241"},
            {"PT115","1234"},
            {"PT116","1235"},
            {"PT117","1242"},
            {"PT118","1243"},
            {"PT119","1244"},
            {"PT120","1245"},
            {"PT121","1237"},
            {"PT122","1238"},
            {"PT123","1295"},
            {"PT124","1296"},
            {"PT125","1233"},
            {"PT126","1236"},
            {"PT127","1239"},
            {"PT139","1247"},
            {"PT140","1246"},
            {"PT141","1248"},
            {"PT142","1297"},
            {"PT143","1298"}
        };
        public static string IfNull(this string String, string NullValue = "") =>
            string.IsNullOrEmpty(String) ? NullValue : String;
        public static string IfNotNull(this string String, string NotNullValue) =>
            !string.IsNullOrEmpty(String) ? NotNullValue : String;
        public static string IfNotNull(this object Obj, string NotNullValue) =>
          Obj != null ? NotNullValue : null;
        public static string MapToExtent(this Extent Extent) =>
            Extent?.Start?.IfNotNull($"p. {Extent.Start}")
            + Extent?.End?.IfNotNull($"-{Extent.End}");
        public static string MapToPart(this List<NamePart> NamePart, string type) =>
            NamePart
            .Where(y => y.Type != null && y.Type.Equals(type))
            .Select(y => y.Text)
            .FirstOrDefault();
        public static string MapToNamePart(this Name Name, string type) =>
            Name.NamePart.MapToPart(type);

        public static ExtractUnit MapToExtractUnit(this Mods x, Concurrent​Dictionary<string, string> UniqueAuthorIds, Concurrent​Dictionary<string, string> UniqueProjectIds)
        => new ExtractUnit
        {
            Authors = x.MapToAuthors().Where(y => UniqueAuthorIds.TryAdd(y.Id, "")).Where(y => !y.Id.Equals("/#//#//#/")),
            Publication = x.MapToPublication(""),
            Publication_Authors = x.MapToPublication_Author("", x.RecordInfo.RecordIdentifier).Where(y => !y.AuthorId.Equals("/#//#//#/")),
            Projects = x.MapToProject(x.RecordInfo.RecordIdentifier).Where(y => UniqueProjectIds.TryAdd(y.Id, ""))
        };

        public static IEnumerable<Author> MapToAuthors(this Mods Mods) =>
            Mods.Name.MapToAuthors();
        public static IEnumerable<Author> MapToAuthors(this IEnumerable<Name> Names) =>
            Names.Select(x => x.MapToAuthor());
        public static Author MapToAuthor(this Name Name) =>
            new Author
            {
                Family = Name.MapToNamePart("family"),
                FamilyPrint = Name.MapToNamePart("family").PrintableLastName(),
                Given = Name.MapToNamePart("given"),
                Id = $"/#/{Name.MapToNamePart("family")}/#/{Name.ID?.Substring(3).IfNull()}/#/{Name.MapToNamePart("given")}",
            };
        public static string MapToId(this IEnumerable<Identifier> Identifiers) =>
            Identifiers
                .Where(y => y.Type.Equals("hdl"))
                .Select(x => x.Text)
                .FirstOrDefault();
        public static string MapToTitle(this TitleInfo TitleInfo) =>
            TitleInfo?.Title.IfNull();

        public static IEnumerable<Publication_Author> MapToPublication_Author(this Mods Mods, string UrlPrefix, string PublicationId) =>
            Mods.Name.MapToPublication_Author(UrlPrefix, PublicationId);
        public static IEnumerable<Publication_Author> MapToPublication_Author(this IEnumerable<Name> Names, string UrlPrefix, string PublicationId) =>
            Names?
                .GroupBy(x => new
                {
                    family = x.NamePart.MapToPart("family"),
                    emplid = x?.ID?.Substring(3).IfNull(),
                    given = x.NamePart.MapToPart("given"),
                })
                .Select((x, i) => new Publication_Author
                {
                    AuthorId = $"/#/{x.Key.family}/#/{x.Key.emplid}/#/{x.Key.given}",
                    //Author = x.First().MapToAuthor(),
                    PublicationId = PublicationId,
                    Emplid = x.Key.emplid,
                    Url = $"{UrlPrefix}{x.Key.emplid}",
                    Sort = i
                });

        public static IEnumerable<Project> MapToProject(this Mods Mods, string PublicationId)
        => Mods.Name.SelectMany(x => x.MapToProject(PublicationId));
        public static IEnumerable<Project> MapToProject(this Name Name, string PublicationId)
        => Name.Affiliation.MapToProject(PublicationId);
        public static IEnumerable<Project> MapToProject(this List<Affiliation> Affiliations, string PublicationId) =>
        Affiliations.Select(x => x.MapToProject(PublicationId));

        public static Project MapToProject(this Affiliation Affiliation, string PublicationId)
        => new Project
        {
            Id = Affiliation.Text.Substring(3),
            PublicationId = PublicationId
        };

        public static string MapToType(this Extension Extension) =>
            IdToType
            .Where(x => x.Key.Equals(Extension.Pubtype.Where(y => y.Src.Equals("ua")).Select(y => y.Text).FirstOrDefault()))
            .Select(x => x.Value)
            .FirstOrDefault();
        public static string MapToTitleInfo(this TitleInfo TitleInfo) =>
            TitleInfo?
            .Title
            .IfNull();
        public static string MapToIdentifier(this Identifier Identifier) =>
            Identifier?
            .Text?
            .IfNotNull($" - ISSN {Identifier.Text}");
        public static string MapToDetail(this IEnumerable<Detail> Detail) =>
            Detail?
            .OrderByDescending(x => x.Type)
            .Select(x => x.Number)
            .ToJoinedString(":")
            .IfNull();
        public static string MapToDate(this string Date) =>
            Date?.IfNotNull($" ({Date}) ");
        public static string MapToPart(this Part Part) =>
            Part?.IfNotNull($"-{Part.Detail.MapToDetail()}{Part.Date.MapToDate()}{Part.Extent.MapToExtent()}");
        public static string MapToRelatedItem(this RelatedItem RelatedItem) =>
            RelatedItem?
            .IfNotNull($"{RelatedItem?.TitleInfo.MapToTitleInfo()}{RelatedItem?.Identifier.MapToIdentifier()}{RelatedItem?.Part.MapToPart()}")
            .IfNull();
        public static string MapToRelatedItem(this IEnumerable<RelatedItem> RelatedItems) =>
            RelatedItems
            .FirstOrDefault(x => x.Type.Equals("host"))
            .MapToRelatedItem();
        public static string MapToPlace(this Place Place) =>
            Place
            .IfNotNull(Place?.PlaceTerm)
            .IfNull();
        public static string MapToOriginInfo(this OriginInfo x) =>
            new[] { x.Place.MapToPlace(), x.Publisher, x.DateIssued }
            .Where(y => !string.IsNullOrEmpty(y))
            .ToJoinedString(", ")
            .IfNull();
        public static string MapToPhysicalDescription(this PhysicalDescription x) =>
            x?.Extent.IfNull();
        public static string MapToExtraInfo(this Mods Mods) =>
            Mods
            .RelatedItem
            .MapToRelatedItem()
            .IfNull($"{Mods.OriginInfo.MapToOriginInfo()},{Mods.PhysicalDescription.MapToPhysicalDescription()}");
        public static IEnumerable<Publication> MapToPublications(this IEnumerable<Mods> Mods, string UrlPrefix) =>
        Mods.Select(x => x.MapToPublication(UrlPrefix));

        public static Publication MapToPublication(this Mods Mods, string UrlPrefix) =>
            new Publication
            {
                Id = Mods.RecordInfo.RecordIdentifier,
                Url = Mods.Identifier.MapToId(),
                Title = Mods.TitleInfo.MapToTitle(),
                Year = Mods.OriginInfo.DateIssued.TryParse(),
                //Authors = Mods.Name.MapToPublication_Author(UrlPrefix, Mods.RecordInfo.RecordIdentifier).ToList(),
                // Projects = Mods.Name.MapToProjects(Mods.RecordInfo.RecordIdentifier).ToList(),
                Type = Mods.Extension.MapToType(),
                ExtraInfo = Mods.MapToExtraInfo()
            };
        /*
                public static IEnumerable<Author> MapToAuthors(this Publication x, HashSet<string> AuthorsUniqFilter)
                    => x
                    .Authors
                    .Select(z => z.Author)
                    .Where(y => y != null && AuthorsUniqFilter.Add(y.Id));
                    */
        public static PublicatieFTS MapToPublicatieFTS(this Publication x)
            => new PublicatieFTS
            {
                Id = x.Id,
                Json = x.ToValue(),
                Year = x.Year
            };
    }
}