// Used as assembly marker.
using System.Text;

namespace Nameless.Gamebuster;

public static class Root {
    public static class HttpRequestHeaders {
        public const string X_FORWARDED_FOR = "X-Forwarded-For";
    }

    public static class HttpResponseHeaders {
        public const string X_JWT_EXPIRED = "X-JWT-Expired";
    }

    public static class Separators {
        public const char SPACE = ' ';
        public const char DASH = '-';
        public const char COMMA = ',';
        public const char SEMICOLON = ';';
        public const char DOT = '.';
        public const char UNDERSCORE = '_';
    }

    internal static class Defaults {
        internal const string EMPTY_STRING = "";

        internal const string JWT_SECRET = "VGhlIG1vb24sIGEgY2VsZXN0aWFsIHBvZXQncyBwZWFybCwgYmF0aGVzIHRoZSBuaWdodCBjYW52YXMgaW4gYW4gZXRoZXJlYWwgZ2xvdywgd2hpc3BlcmluZyBhbmNpZW50IHNlY3JldHMgdG8gdGhlIHN0YXJnYXplcidzIHNvdWwsIGFuIGV0ZXJuYWwgZGFuY2Ugb2YgbGlnaHQgdGhhdCB3ZWF2ZXMgZHJlYW1zIGFjcm9zcyB0aGUgY29zbWljIHRhcGVzdHJ5Lg==";

        internal static Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}
