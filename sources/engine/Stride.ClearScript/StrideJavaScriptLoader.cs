using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Stride.Core.IO;

namespace Stride.ClearScript
{
    internal static class MiscHelpers
    {
        public static bool Try<T>(out T result, Func<T> func)
        {
            try
            {
                result = func();
                return true;
            }
            catch (Exception)
            {
                result = default;
                return false;
            }
        }

        public static void VerifyNonNullArgument(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        private static readonly char[] searchPathSeparators = { ';' };
        public static IEnumerable<string> SplitSearchPath(this string searchPath)
        {
            return searchPath.Split(searchPathSeparators, StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.OrdinalIgnoreCase);
        }

        public static byte[] ReadToEnd(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
        internal class StrideJavaScriptLoader : DocumentLoader
    {
        private static readonly IReadOnlyCollection<string> relativePrefixes = new List<string>
            {
                "." + Path.DirectorySeparatorChar,
                "." + Path.AltDirectorySeparatorChar,
                ".." + Path.DirectorySeparatorChar,
                ".." + Path.AltDirectorySeparatorChar,
            };

        private static bool TryCombineSearchUri(Uri searchUri, string specifier, out Uri uri)
        {
            var searchUrl = searchUri.AbsoluteUri;
            if (!searchUrl.EndsWith("/", StringComparison.Ordinal))
            {
                searchUri = new Uri(searchUrl + "/");
            }

            return Uri.TryCreate(searchUri, specifier, out uri);
        }
        private static Uri GetBaseUri(DocumentInfo sourceInfo)
        {
            var sourceUri = sourceInfo.Uri;

            if ((sourceUri == null) && !Uri.TryCreate(sourceInfo.Name, UriKind.RelativeOrAbsolute, out sourceUri))
            {
                return null;
            }

            if (!sourceUri.IsAbsoluteUri)
            {
                return null;
            }

            return sourceUri;
        }

        private static bool SpecifierMayBeRelative(DocumentSettings settings, string specifier)
        {
            return !settings.AccessFlags.HasFlag(DocumentAccessFlags.EnforceRelativePrefix) || relativePrefixes.Any(specifier.StartsWith);
        }

        private static IEnumerable<Uri> GetRawUris(DocumentSettings settings, DocumentInfo? sourceInfo, string specifier)
        {
           // yield return new Uri("/local/", specifier);

           Uri baseUri;
            Uri uri;

            if (sourceInfo.HasValue && SpecifierMayBeRelative(settings, specifier))
            {
                baseUri = GetBaseUri(sourceInfo.Value);
                if ((baseUri != null) && Uri.TryCreate(baseUri, specifier, out uri))
                {
                    yield return uri;
                }
            }

            var searchPath = settings.SearchPath;
            if (!string.IsNullOrWhiteSpace(searchPath))
            {
                foreach (var url in searchPath.SplitSearchPath())
                {
                    if (Uri.TryCreate(url, UriKind.Relative, out baseUri) && TryCombineSearchUri(baseUri, specifier, out uri))
                    {
                        yield return uri;
                    }
                }
            }

            if (MiscHelpers.Try(out var path, () => Path.Combine("/local/", specifier)) && Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out uri))
            {
                yield return uri;
            }

            if (MiscHelpers.Try(out path, () => Path.Combine("/local/", specifier)) && Uri.TryCreate(path, UriKind.Relative, out uri))
            {
                yield return uri;
            }

            using (var process = Process.GetCurrentProcess())
            {
                var module = process.MainModule;
                if ((module != null) && Uri.TryCreate(module.FileName, UriKind.Relative, out baseUri) && Uri.TryCreate(baseUri, specifier, out uri))
                {
                    yield return uri;
                }
            }
        }

        public override uint MaxCacheSize { get; set; }

        public override async Task<Document> LoadDocumentAsync(DocumentSettings settings, DocumentInfo? sourceInfo, string specifier, DocumentCategory category, DocumentContextCallback contextCallback)
        {
            
            //MiscHelpers.VerifyNonNullArgument(settings, nameof(settings));
           // MiscHelpers.VerifyNonBlankArgument(specifier, nameof(specifier), "Invalid document specifier");

            if ((settings.AccessFlags & DocumentAccessFlags.EnableAllLoading) == DocumentAccessFlags.None)
            {
                throw new UnauthorizedAccessException("The script engine is not configured for loading documents");
            }

            if (category == null)
            {
                category = sourceInfo.HasValue ? sourceInfo.Value.Category : DocumentCategory.Script;
            }

            (Document Document, List<Uri> CandidateUris) result;

            //if (Uri.TryCreate(specifier, UriKind.RelativeOrAbsolute, out var uri) && uri.IsAbsoluteUri)
            //{
            //    result = await GetCachedDocumentOrCandidateUrisAsync(settings, sourceInfo, uri).ConfigureAwait(false);
            //}
            //else
            //{
            //    result = await GetCachedDocumentOrCandidateUrisAsync(settings, sourceInfo, specifier).ConfigureAwait(false);
            //}

            //if (result.Document != null)
            //{
            //    return result.Document;
            //}

            //if (result.CandidateUris.Count < 1)
            //{
            //    throw new FileNotFoundException(null, specifier);
            //}

            //if (result.CandidateUris.Count == 1)
            //{
            var url = GetRawUris(settings, sourceInfo, specifier).FirstOrDefault();
                return await LoadDocumentAsync(settings, url, category, contextCallback).ConfigureAwait(false);
            //}

            //var exceptions = new List<Exception>(result.CandidateUris.Count);

            //foreach (var candidateUri in result.CandidateUris)
            //{
            //    var task = LoadDocumentAsync(settings, candidateUri, category, contextCallback);
            //    try
            //    {
            //        return await task.ConfigureAwait(false);
            //    }
            //    catch (Exception exception)
            //    {
            //        if ((task.Exception != null) && task.Exception.InnerExceptions.Count == 1)
            //        {
            //            Debug.Assert(ReferenceEquals(task.Exception.InnerExceptions[0], exception));
            //            exceptions.Add(exception);
            //        }
            //        else
            //        {
            //            exceptions.Add(task.Exception);
            //        }
            //    }
            //}

            //if (exceptions.Count < 1)
            //{
            //    MiscHelpers.AssertUnreachable();
            //    throw new FileNotFoundException(null, specifier);
            //}

            //if (exceptions.Count == 1)
            //{
            //    MiscHelpers.AssertUnreachable();
            //    throw new FileLoadException(exceptions[0].Message, specifier, exceptions[0]);
            //}

            //throw new AggregateException(exceptions).Flatten();
        }

        private async Task<Document> LoadDocumentAsync(DocumentSettings settings, Uri uri, DocumentCategory category, DocumentContextCallback contextCallback)
        {
            //if (uri.IsFile)
            //{
            //    if (!settings.AccessFlags.HasFlag(DocumentAccessFlags.EnableFileLoading))
            //    {
            //        throw new UnauthorizedAccessException("The script engine is not configured for loading documents from the file system");
            //    }
            //}
            //else
            //{
            //    if (!settings.AccessFlags.HasFlag(DocumentAccessFlags.EnableWebLoading))
            //    {
            //        throw new UnauthorizedAccessException("The script engine is not configured for downloading documents from the Web");
            //    }
            //}

            var cachedDocument = GetCachedDocument(uri);
            if (cachedDocument != null)
            {
                return cachedDocument;
            }

            string contents;

            //if (uri.IsFile)
            {
               
                using (var stream = VirtualFileSystem.OpenStream(uri.OriginalString+"."+settings.FileNameExtensions, VirtualFileMode.Open, VirtualFileAccess.Read))
                using (var streamReader = new StreamReader(stream))
                {
                    //read the raw asset content
                    contents = await streamReader.ReadToEndAsync();
                }
            }
            //else
            //{
            //    using (var client = new WebClient())
            //    {
            //        contents = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
            //    }
            //}

            var documentInfo = new DocumentInfo(uri) { Category = category, ContextCallback = contextCallback };

            var callback = settings.LoadCallback;
            callback?.Invoke(ref documentInfo);

            return CacheDocument(new StringDocument(documentInfo, contents), false);
        }

        private readonly List<Document> cache = new List<Document>();
        public override Document CacheDocument(Document document, bool replace)
        {
            MiscHelpers.VerifyNonNullArgument(document, nameof(document));
            if (!document.Info.Uri.IsAbsoluteUri)
            {
                throw new ArgumentException("The document must have an absolute URI");
            }

            lock (cache)
            {
                for (var index = 0; index < cache.Count;)
                {
                    var cachedDocument = cache[index];
                    if (cachedDocument.Info.Uri != document.Info.Uri)
                    {
                        index++;
                    }
                    else
                    {
                        if (!replace)
                        {
                            Debug.Assert(cachedDocument.Contents.ReadToEnd().SequenceEqual(document.Contents.ReadToEnd()));
                            return cachedDocument;
                        }

                        cache.RemoveAt(index);
                    }
                }

                var maxCacheSize = Math.Max(16, Convert.ToInt32(Math.Min(MaxCacheSize, int.MaxValue)));
                while (cache.Count >= maxCacheSize)
                {
                    cache.RemoveAt(cache.Count - 1);
                }

                cache.Insert(0, document);
                return document;
            }
        }

        public override void DiscardCachedDocuments()
        {
            lock (cache)
            {
                cache.Clear();
            }
        }
    }
}
