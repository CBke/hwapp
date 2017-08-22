using Models;
using Extentions;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System;

namespace Data
{
    public static class DataMapper
    {
        public static Dictionary<string, string> IdToType = new Dictionary<string, string>
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

        public static string ToExtent(this Extent Extent)
        => Extent?.Start?.IfNotNull($"p. {Extent.Start}") + Extent?.End?.IfNotNull($"-{Extent.End}");
        public static string ToPart(this List<NamePart> NamePart, string type)
        => NamePart
            .Where(y => y.Type != null && y.Type.Equals(type))
            .Select(y => y.Text)
            .FirstOrDefault();
        public static string ToPrintableLastName(this string LastName)
        => LastName?.ToPrintableLastName(LastName.IndexOf(", ", StringComparison.Ordinal));
        public static string ToPrintableLastName(this string LastName, int index)
        => (index <= 0) ? LastName : $"{LastName.Substring(index + 2)} { LastName.Substring(0, index)}";
        public static string ToNamePart(this Name Name, string type)
        => Name.NamePart.ToPart(type);
        public static ExtractUnit ToExtractUnit(this Mods x, Concurrent​Dictionary<string, string> UniqueAuthorIds, Concurrent​Dictionary<string, string> UniqueProjectIds)
        => new ExtractUnit
        {
            Authors = x.ToAuthors().Where(y => UniqueAuthorIds.TryAdd(y.Id, "")).Where(y => !y.Id.Equals("/#//#//#/")),
            Publication = x.ToPublication(""),
            Publication_Authors = x.ToPublication_Author("", x.RecordInfo.RecordIdentifier).Where(y => !y.AuthorId.Equals("/#//#//#/")),
            Projects = x.ToProject(x.RecordInfo.RecordIdentifier).Where(y => UniqueProjectIds.TryAdd(y.Id, ""))
        };
        public static IEnumerable<Author> ToAuthors(this Mods Mods)
        => Mods.Name.ToAuthors();
        public static IEnumerable<Author> ToAuthors(this IEnumerable<Name> Names)
        => Names.Select(x => x.ToAuthor());
        public static Author ToAuthor(this Name Name)
        => new Author
        {
            Family = Name.ToNamePart("family"),
            FamilyPrint = Name.ToNamePart("family").ToPrintableLastName().IfNull(),
            Given = Name.ToNamePart("given"),
            Id = $"/#/{Name.ToNamePart("family")}/#/{Name.ID?.Substring(3).IfNull()}/#/{Name.ToNamePart("given")}",
            Emplid = Name.ToEmplid()
        };
        public static string ToId(this IEnumerable<Identifier> Identifiers)
        => Identifiers
            .Where(y => y.Type.Equals("hdl"))
            .Select(x => x.Text)
            .FirstOrDefault();
        public static string ToTitle(this TitleInfo TitleInfo)
        => TitleInfo?.Title.IfNull();
        public static IEnumerable<Publication_Author> ToPublication_Author(this Mods Mods, string UrlPrefix, string PublicationId)
        => Mods.Name.ToPublication_Author(UrlPrefix, PublicationId);
        public static IEnumerable<Publication_Author> ToPublication_Author(this IEnumerable<Name> Names, string UrlPrefix, string PublicationId)
        => Names?
        .GroupBy(x => new
        {
            family = x.NamePart.ToPart("family"),
            emplid = x.ToEmplid(),
            given = x.NamePart.ToPart("given"),
        })
        .Select((x, i) => new Publication_Author
        {
            AuthorId = $"/#/{x.Key.family}/#/{x.Key.emplid}/#/{x.Key.given}",
            PublicationId = PublicationId,
            Url = $"{UrlPrefix}{x.Key.emplid}",
            Sort = i
        });
        public static string ToEmplid(this Name Name)
        => Name?.ID?.Substring(3).IfNull();
        public static IEnumerable<Project> ToProject(this Mods Mods, string PublicationId)
        => Mods.Name.SelectMany(x => x.ToProject(PublicationId));
        public static IEnumerable<Project> ToProject(this Name Name, string PublicationId)
        => Name.Affiliation.ToProject(PublicationId);
        public static IEnumerable<Project> ToProject(this List<Affiliation> Affiliations, string PublicationId) =>
        Affiliations.Select(x => x.ToProject(PublicationId));
        public static Project ToProject(this Affiliation Affiliation, string PublicationId)
        => new Project
        {
            Id = Affiliation.Text.Substring(3),
            PublicationId = PublicationId
        };
        public static string ToType(this Extension Extension)
        => IdToType
            .Where(x => x.Key.Equals(Extension.Pubtype.Where(y => y.Src.Equals("ua")).Select(y => y.Text).FirstOrDefault()))
            .Select(x => x.Value)
            .FirstOrDefault();
        public static string ToTitleInfo(this TitleInfo TitleInfo)
        => TitleInfo?
            .Title
            .IfNull();
        public static string ToIdentifier(this Identifier Identifier)
         => Identifier?
            .Text?
            .IfNotNull($" - ISSN {Identifier.Text}");
        public static string ToDetail(this IEnumerable<Detail> Detail)
        => Detail?
            .OrderByDescending(x => x.Type)
            .Select(x => x.Number)
            .ToJoinedString(":")
            .IfNull();
        public static string ToDate(this string Date)
        => Date?.IfNotNull($" ({Date}) ");
        public static string ToPart(this Part Part)
        => Part?.IfNotNull($"-{Part.Detail.ToDetail()}{Part.Date.ToDate()}{Part.Extent.ToExtent()}");
        public static string ToRelatedItem(this RelatedItem RelatedItem)
        => RelatedItem?
            .IfNotNull($"{RelatedItem?.TitleInfo.ToTitleInfo()}{RelatedItem?.Identifier.ToIdentifier()}{RelatedItem?.Part.ToPart()}")
            .IfNull();
        public static string ToRelatedItem(this IEnumerable<RelatedItem> RelatedItems)
        => RelatedItems
            .FirstOrDefault(x => x.Type.Equals("host"))
            .ToRelatedItem();
        public static string ToPlace(this Place Place)
        => Place
            .IfNotNull(Place?.PlaceTerm)
            .IfNull();
        public static string ToOriginInfo(this OriginInfo x)
        => new[] { x.Place.ToPlace(), x.Publisher, x.DateIssued }
            .Where(y => !string.IsNullOrEmpty(y))
            .ToJoinedString(", ")
            .IfNull();
        public static string ToPhysicalDescription(this PhysicalDescription x)
        => x?.Extent.IfNull();
        public static string ToExtraInfo(this Mods Mods)
        => Mods
            .RelatedItem
            .ToRelatedItem()
            .IfNull($"{Mods.OriginInfo.ToOriginInfo()},{Mods.PhysicalDescription.ToPhysicalDescription()}");
        public static Publication ToPublication(this Mods Mods, string UrlPrefix)
        => new Publication
        {
            Id = Mods.RecordInfo.RecordIdentifier,
            Url = Mods.Identifier.ToId(),
            Title = Mods.TitleInfo.ToTitle(),
            Year = Mods.OriginInfo.DateIssued.TryParse(),
            Type = Mods.Extension.ToType(),
            ExtraInfo = Mods.ToExtraInfo()
        };
        public static PublicatieFTS ToPublicatieFTS(this Publication x)
        => new PublicatieFTS
        {
            Id = x.Id,
            Json = x.ToValue(),
            Year = x.Year
        };
    }
}