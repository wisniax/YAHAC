using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;

namespace ITR
{
    /// <summary>
    /// ItemTextureResolver is class used to match Item to HyPixel_ID<br/>
    /// and show it's texture from vanilla or loaded Resourcepacks.<br/>
    /// Also capable to return Bukkit material texture<br/>if HyPixel_ID texture
    /// is not found.<br/><br/>
    /// Initialize with  <c>.Init()</c>  before using!!!
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Style nazewnictwa", Justification = "Deserialization From JSON requires names to exact match")]
    public partial class ItemTextureResolver
    {
        private readonly HttpClient http;

        private readonly ConcurrentQueue<HyItems_Item> skullDownloadQueue;

        private readonly List<VanillaID> itemsIDTable;
        private readonly SortedDictionary<Material, List<Cit_Item>> citDict;
        private readonly SortedDictionary<string, HyItems_Item> hyItemsDict;

        readonly object citDict_Lock;

        private string pathToCacheFile;

        bool noFileCache = false;

        private bool _initialized = false;
        /// <summary>
        /// Indicates that this instance is ready to use
        /// </summary>
        public bool Initialized { get => _initialized; }

        /// <summary>
        /// When RegisterOverride is true RegisterItem will override hyPixel item definition,<br/>
        /// else only add resource pack item
        /// </summary>
        public bool RegisterOverride { get; set; }

        private readonly List<string> _resourcepackPriority;

        public string ResourcepackPriority
        {
            get
            {
                var ret = string.Empty;
                foreach (var item in _resourcepackPriority)
                {
                    ret += item + ";";
                }
                return ret;
            }
        }

        public bool ResourcepackPrioritySet(string partialName, int newPriority)
        {
            var search = _resourcepackPriority.FindIndex(x => { return x.Contains(partialName.ToLower()); });
            if (search == -1) return false;

            var myLovedOne = _resourcepackPriority[search];
            _resourcepackPriority.RemoveAt(search);
            _resourcepackPriority.Insert(newPriority, myLovedOne);
            return true;
        }

        System.Threading.Thread downloader;

        public delegate void DownloadedItemHandler(ItemTextureResolver source, Item itemUpdated);
        public event DownloadedItemHandler DownloadedItemEvent;

        private void OnDownloadedItem(Item item)
        {
            DownloadedItemEvent?.Invoke(this, item);
        }

        /// <summary>
        /// New ItemTextureResolver with own resources<br/>
        /// Remember to initialize before use!
        /// </summary>
        public ItemTextureResolver()
        {
            downloader = new(SkullDownloader)
            {
                IsBackground = true
            };
            citDict_Lock = new();
            skullDownloadQueue = new();
            http = new();
            citDict = new();
            hyItemsDict = new();
            itemsIDTable = JsonSerializer.Deserialize<List<VanillaID>>(Properties.Resources.ItemsJSON);

            foreach (var material in Enum.GetValues<Material>())
            {
                if (!citDict.ContainsKey(material))
                    citDict.Add(material, new List<Cit_Item>());
            }
            _resourcepackPriority = new()
            {
                "HyPixelSkull".ToLower(),
                "Manual".ToLower(),
                "Vanilla".ToLower()
            };
            RegisterOverride = false;
        }

        void SkullDownloader()
        {
            while (!skullDownloadQueue.IsEmpty)
            {
                skullDownloadQueue.TryDequeue(out var item);
                Cit_Item cit = new()
                {
                    HyPixel_ID = item.id
                };

                var JSON = JsonDocument.Parse(
                    Convert.FromBase64String(
                        item.skin
                        .Replace(@"\u003d", "=")
                        //bo to jest Base64 ale zurlowany a to nie umie odurlować samo
                        .Replace('-', '+')
                        .Replace('_', '/')
                        .PadRight(4 * ((item.skin.Length + 3) / 4), '=')));
                string skin = JSON.RootElement
                    .GetProperty("textures")
                    .GetProperty("SKIN")
                    .GetProperty("url").GetString()
                    .Split('/')[^1];

                var responde = http.GetAsync($"https://mc-heads.net/head/{skin}/64");

                cit.Texture = new();
                responde.Result.Content.ReadAsStream().CopyTo(cit.Texture);

                cit.Texture.Seek(0, SeekOrigin.Begin);
                ImageProcessor.ImageFactory imageFactory = new();
                imageFactory.Load(cit.Texture);
                imageFactory.BackgroundColor(Color.Transparent);
                imageFactory.Format(new ImageProcessor.Imaging.Formats.PngFormat());
                imageFactory.Save(cit.Texture);

                cit.ResourcepackName = "HyPixelSkull";

                lock (citDict_Lock)
                {
                    citDict[Enum.Parse<Material>(item.material)].Add(cit);
                }


                CacheAppend($"Skins/[{item.material}]{cit.HyPixel_ID}.png", cit.Texture.ToArray());
                var itemClass = GetItemFromID(item.id);
                if (itemClass == null) throw new Exception("Unexpected error: Texture was downloaded for item that appear to not exist. REPORT THIS! addicional info:" + item.id);
                OnDownloadedItem(itemClass);
            }
        }

        public void MakeSafeToQuit()
        {
            skullDownloadQueue.Clear();
            downloader.Join(10000);
        }

        private void CacheAppend(string entry, Span<Byte> data)
        {
            if (noFileCache) return;

            Directory.CreateDirectory(Path.GetDirectoryName(pathToCacheFile));
            using var cacheFileStream = new FileStream(pathToCacheFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);


            using ZipArchive cacheArchive = new(cacheFileStream, ZipArchiveMode.Update, true);

            var zipEntry = cacheArchive.CreateEntry(entry).Open();
            zipEntry.Write(data);
        }

        /// <summary>
        /// Use this to redownload(refresh) HyPixel Items
        /// </summary>
        public void RefreshHyItems()
        {
            //download items
            var responde = http.GetAsync("https://api.hypixel.net/resources/skyblock/items");
            var content = responde.Result.Content.ReadAsStream();
            var text = new StreamReader(content).ReadToEnd();

            //deserialize Items
            var hyItems_JSON = JsonSerializer.Deserialize<HyItems>(text);
            foreach (var item in hyItems_JSON.items)
            {
                if (hyItemsDict.ContainsKey(item.id))
                {
                    hyItemsDict[item.id] = item;
                }
                else
                {
                    hyItemsDict.Add(item.id, item);
                }
            }
        }

        /// <summary>
        /// Fast Initialization before use of <c>ItemTextureResolver</c><br/>
        /// Items data is loaded form HyPixel API<br/>
        /// FastInit compared to Init do not download all skull items,<br/>
        /// instead they are downloaded on deamand and stored to cache. 
        /// </summary>
        /// <param name="pathToCacheFile">Relative path to ITR cache, where downoaded data is stored</param>
        /// <param name="noFileCache">if true, cache file is not created. Not recomended, API is slow</param>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public void FastInit(string pathToCacheFile = @".\ITR_Cache.zip", bool noFileCache = false)
        {
            this.noFileCache = noFileCache;
            this.pathToCacheFile = pathToCacheFile;
            bool useLocalCache = File.Exists(pathToCacheFile);

            HyItems_Item barrier = new()
            {
                id = "BARRIER",
                material = "BARRIER",
                name = "Barrier Block"
            };
            hyItemsDict.Add(barrier.id, barrier);

            RefreshHyItems();

            //download or unzip skull items
            if (useLocalCache)
            {
                using var cacheFileStream = new FileStream(pathToCacheFile, FileMode.Open);
                using ZipArchive cacheArchive = new(cacheFileStream, ZipArchiveMode.Read);

                foreach (var entry in cacheArchive.Entries)
                {
                    if (!entry.Name.EndsWith(".png")) continue;

                    Cit_Item cit = new();

                    var opB = entry.Name.IndexOf('[');
                    var clB = entry.Name.IndexOf(']');

                    var material = entry.Name.Substring(opB + 1, clB - opB - 1);
                    cit.HyPixel_ID = entry.Name.Substring(clB + 1, entry.Name.IndexOf('.') - clB - 1);

                    using (var img = entry.Open())
                    {
                        cit.Texture = new();
                        img.CopyTo(cit.Texture);
                    }

                    cit.ResourcepackName = "HyPixelSkull";

                    citDict[Enum.Parse<Material>(material)].Add(cit);
                }
            }

            ZipArchive vanillaTex = new(new MemoryStream(Properties.Resources.itemsZIP));
            foreach (var item in itemsIDTable)
            {
                Cit_Item cit = new()
                {
                    Texture = new(),
                    HyPixel_ID = ((Material)item.type).ToString()
                };
                if (item.meta != 0)
                    cit.HyPixel_ID += ":" + item.meta;

                vanillaTex.GetEntry($"{item.type}-{item.meta}.png").Open().CopyTo(cit.Texture);
                cit.ResourcepackName = "Vanilla";
                citDict[(Material)item.type].Add(cit);
            }

            _initialized = true;
        }

        /// <summary>
        /// Loads Resourcepack from .zip and catalogs it
        /// </summary>
        /// <param name="file">path to Resourcepack .zip file</param>
        /// <returns>number of loaded and cataloged textures</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
        public int LoadResourcepack(string file)
        {
            var plik = File.OpenRead(file);
            byte[] bytes = new byte[plik.Length];
            plik.Read(bytes);
            return LoadResourcepack(bytes);
        }


#pragma warning disable CS1584 // Komentarz XML zawiera składniowo niepoprawny atrybut cref
        /// <summary>
        /// Loads Resourcepack from .zip and catalogs it
        /// </summary>
        /// <param name="bytes">Resourcepack file bytes</param>
        /// <returns>number of loaded and cataloged textures</returns>
        /// <exception cref="Nie Ma Wyjątków Od Jebania Disa"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="IOException"></exception>
#pragma warning restore CS1584 // Komentarz XML zawiera składniowo niepoprawny atrybut cref
        public int LoadResourcepack(byte[] bytes)
        {
            int loaded = 0;

            ZipArchive zip = new(new MemoryStream(bytes));

            var ResourcepackName = zip.Entries[0].FullName.TrimEnd('/');

            foreach (var entry in zip.Entries)
            {
                if (!entry.Name.Contains(".properties")) continue;
                var path = entry.FullName[..(entry.FullName.Length - entry.Name.Length)];
                var propFile = new StreamReader(entry.Open());

                string line;
                Cit_Item cit_Item = new();
                Material material = Material.AIR;
                string texturePath = string.Empty;
                cit_Item.ResourcepackName = ResourcepackName;

                while ((line = propFile.ReadLine()) != null)
                {
                    string[] lineSplit = line.Split('=');
                    switch (lineSplit[0])
                    {
                        case "items":
                            material = (Material)itemsIDTable.Find(
                                new Predicate<VanillaID>(
                                    x =>
                                    {
                                        if (lineSplit[1].Contains(':'))
                                        {
                                            return x.text_type == lineSplit[1].Split(':')[1];
                                        }
                                        else
                                        {
                                            return x.type.ToString() == lineSplit[1];
                                        }
                                    })).type;
                            break;

                        case "nbt.display.Name":
                            if (lineSplit[1].Contains("iregex"))
                            {
                                //RegEx to czarna magia, nie baw się w to
                                break;
                            }

                            if (lineSplit[1].Contains('*'))
                            {
                                cit_Item.Name_pattern = lineSplit[1].Split('*')[1];
                            }
                            else
                            {
                                cit_Item.Name_pattern = lineSplit[1];
                            }
                            break;

                        case "nbt.ExtraAttributes.id":
                            if (lineSplit[1].Contains("iregex"))
                            {
                                //RegEx to czarna magia, nie baw się w to
                                break;
                            }
                            cit_Item.HyPixel_ID = lineSplit[1];
                            break;

                        case "model":
                            if (lineSplit[1].Contains("iregex"))
                            {
                                //RegEx to czarna magia, nie baw się w to
                                break;
                            }
                            if (zip.GetEntry(path + lineSplit[1] + ".gif") != null)
                            {
                                texturePath = path + lineSplit[1] + ".gif";
                            }
                            else if (zip.GetEntry(path + lineSplit[1] + ".png") != null)
                            {
                                texturePath = path + lineSplit[1] + ".png";
                            }
                            break;

                        case "texture":

                            //tak
                            var image = path + lineSplit[1].Split('/')[^1];
                            if (!image.Contains(".png"))
                            {
                                if (zip.GetEntry(image + ".gif") != null)
                                {
                                    image += ".gif";
                                }
                                else
                                if (zip.GetEntry(image + ".png") != null)
                                {
                                    image += ".png";
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (zip.GetEntry(image) != null)
                                texturePath = image;
                            break;

                        default:
                            break;
                    }
                }


                //if texturePath is null, texture should be named same as folder so go check it
                if (texturePath == string.Empty && !entry.Name.Contains("name") && !entry.Name.Contains("bazaar"))
                {
                    //last hope for our boyyy
                    var lastHopeImage = path + entry.Name.Split('.')[0];
                    if (zip.GetEntry(lastHopeImage + ".gif") != null)
                    {
                        texturePath = lastHopeImage + ".gif";
                    }
                    else
                    if (zip.GetEntry(lastHopeImage + ".png") != null)
                    {
                        texturePath = lastHopeImage + ".png";
                    }
                }

                if (texturePath == string.Empty) continue;

                cit_Item.Texture = new();
                zip.GetEntry(texturePath).Open().CopyTo(cit_Item.Texture);


                if (texturePath.Contains(".png", StringComparison.CurrentCultureIgnoreCase) && zip.GetEntry(texturePath + ".mcmeta") != null)
                {
                    var animMcMeta = JsonDocument.Parse(zip.GetEntry(texturePath + ".mcmeta").Open());
                    var frametime = animMcMeta.RootElement.GetProperty("animation").GetProperty("frametime").GetInt32() * 50;
                    bool verticalGif;
                    int frameResolution;

                    cit_Item.Texture.Seek(0, SeekOrigin.Begin);
                    var workingImage = new ImageProcessor.ImageFactory();
                    workingImage.Load(cit_Item.Texture);



                    if (workingImage.Image.Size.Width > workingImage.Image.Size.Height)
                    {
                        verticalGif = false;
                        frameResolution = workingImage.Image.Size.Height;
                    }
                    else
                    {
                        verticalGif = true;
                        frameResolution = workingImage.Image.Size.Width;
                    }

                    var gifMakeItHappener = new ImageProcessor.Imaging.Formats.GifEncoder(frameResolution, frameResolution);

                    int startingPoint = 0;
                    while (startingPoint < (verticalGif ? workingImage.Image.Size.Height : workingImage.Image.Size.Width))
                    {
                        workingImage.Crop(new Rectangle(
                            verticalGif ? 0 : startingPoint,
                            verticalGif ? startingPoint : 0,
                            frameResolution,
                            frameResolution));
                        var frame = new ImageProcessor.Imaging.Formats.GifFrame
                        {
                            Delay = new TimeSpan(0, 0, 0, 0, frametime),
                            Image = workingImage.Image
                        };
                        gifMakeItHappener.AddFrame(frame);
                        startingPoint += frameResolution;
                        workingImage.Reset();
                    }
                    gifMakeItHappener.Save(cit_Item.Texture);

                }

                citDict[material].Add(cit_Item);

                if (!_resourcepackPriority.Contains(cit_Item.ResourcepackName.ToLower()))
                {
                    _resourcepackPriority.Add(cit_Item.ResourcepackName.ToLower());
                    ResourcepackPrioritySet(cit_Item.ResourcepackName.ToLower(), 0);
                }

                loaded++;
            }

            return loaded;
        }

        public HyItems_Item? GetHyItemDataFromID(string hyPixel_ID)
        {
            if (hyItemsDict.TryGetValue(hyPixel_ID, out HyItems_Item value))
            {
                return value;
            }
            else
            {
                return null;
            }    
        }

        /// <summary>
        /// Look for item by HyPixel_ID<br/>
        /// Note: this is threadsafe (at least is in theory :D)
        /// </summary>
        /// <param name="hyPixel_ID"></param>
        /// <returns>Item with mathing HyPixel_ID; if item is not found returns <c>null</c></returns>
        /// <exception cref="ITRNotInitializedException"></exception>
        public Item GetItemFromID(string hyPixel_ID)
        {

            lock (citDict_Lock)
            {
                if (hyPixel_ID.Contains("ENCHANTMENT"))
                {
                    Cit_Item citBack = new();

                    foreach (var resourcepack in _resourcepackPriority)
                    {
                        citBack = citDict[Material.ENCHANTED_BOOK].Find(x => x.ResourcepackName.ToLower() == resourcepack);
                        if (citBack.HyPixel_ID != null) break;
                    }

                    MemoryStream textureRet = new();
                    citBack.Texture.Seek(0, SeekOrigin.Begin);
                    citBack.Texture.CopyTo(textureRet);

                    Item retItem = new(hyPixel_ID, hyPixel_ID, Material.ENCHANTED_BOOK, true, textureRet, true, 0, "ENCHANTMENT",
                       Rarity.Very_Special, 0, true, citBack.ResourcepackName);
                    return retItem;
                }
                else
                if (hyItemsDict.TryGetValue(hyPixel_ID, out HyItems_Item value))
                {
                    bool isOriginalTexture = true;
                    Item retItem;

                    Material material = Enum.Parse<Material>(value.material);
                    //Dark Oak Wood

                    HyItems_Item citTestTestant = value;
                    Predicate<Cit_Item> citTest = new(x => x.HyPixel_ID == citTestTestant.id || (x.Name_pattern != null && citTestTestant.name.Contains(x.Name_pattern, StringComparison.InvariantCultureIgnoreCase)));


                    Cit_Item citBack = new();

                    var allMatches = citDict[material].FindAll(citTest);

                    if (allMatches.Count != 0)
                        foreach (var resourcepack in _resourcepackPriority)
                        {
                            citBack = allMatches.Find(x => x.ResourcepackName.ToLower() == resourcepack);
                            if (citBack.HyPixel_ID != null) break;
                        }

                    //if not found
                    if (citBack.HyPixel_ID == null)
                    {
                        citTestTestant.id = value.material + (value.durability != 0 ? ":" + value.durability.ToString() : "");
                        int citIdx = citDict[material].FindIndex(citTest);

                        if (allMatches.Count != 0)
                            foreach (var resourcepack in _resourcepackPriority)
                            {
                                citBack = allMatches.Find(x => x.ResourcepackName.ToLower() == resourcepack);
                                if (citBack.HyPixel_ID != null) break;
                            }

                        if (material == Material.SKULL_ITEM && value.durability == 3)
                        {
                            var isAlready = false;
                            foreach (var item in skullDownloadQueue.ToArray())
                            {
                                if (item.id == hyPixel_ID) { isAlready = true; break; }
                            }

                            if (!isAlready) skullDownloadQueue.Enqueue(hyItemsDict[hyPixel_ID]);
                            if (!downloader.IsAlive)
                            {
                                downloader = new(SkullDownloader)
                                {
                                    IsBackground = true
                                };
                                downloader.Start();
                            }

                        }

                        isOriginalTexture = false;
                        citBack = citDict[material][Math.Max(0, citIdx)];
                    }

                    MemoryStream textureRet = new();
                    citBack.Texture.Seek(0, SeekOrigin.Begin);
                    citBack.Texture.CopyTo(textureRet);

                    if (value.color != null && !isOriginalTexture)
                    {
                        var colorString = value.color.Split(',');
                        Color color = Color.FromArgb(Convert.ToInt32(colorString[0]), Convert.ToInt32(colorString[1]), Convert.ToInt32(colorString[2]));
                        textureRet.Seek(0, SeekOrigin.Begin);
                        using ImageProcessor.ImageFactory imageFactory = new();
                        imageFactory.Load(textureRet);
                        imageFactory.Format(new ImageProcessor.Imaging.Formats.PngFormat());
                        imageFactory.ReplaceColor(Color.FromArgb(198, 92, 53), color, 64);
                        imageFactory.Brightness(-13);
                        imageFactory.Save(textureRet);
                    }

                    retItem = new(value.name, value.id, material, isOriginalTexture, textureRet, hyItemsDict[hyPixel_ID].glowing, value.durability, value.category,
                        Item.GetRarityFromString(value.tier), value.npc_sell_price, (value.unstackable != null && value.unstackable == true), citBack.ResourcepackName);
                    return retItem;
                }
                if (!Initialized)
                {
                    throw (new ITRNotInitializedException());
                }
                return null;
            }


        }

        /// <summary>
        /// Registers new item as resourcepack "Manual".
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="texture"></param>
        public void RegisterItem(HyItems_Item itemData, MemoryStream texture = null)
        {

            Cit_Item cit = new();
            cit.Texture = new();
            cit.HyPixel_ID = itemData.id;
            cit.ResourcepackName = "Manual";
            if (texture != null)
            {
                cit.Texture = new();
                texture.Seek(0, SeekOrigin.Begin);
                texture.CopyTo(cit.Texture);
                cit.Texture.Seek(0, SeekOrigin.Begin);
            }
            var material = Enum.Parse<Material>(itemData.material);

            lock (citDict_Lock)
            {
                if (material == Material.SKULL_ITEM && itemData.durability == 3)
                {
                    var oldIdx = citDict[material].FindIndex(x => x.HyPixel_ID == itemData.id);
                    citDict[material].RemoveAt(oldIdx);
                }
                else
                {
                    citDict[material].Add(cit);
                }
            }

            if (hyItemsDict.ContainsKey(itemData.id))
            {
                if (RegisterOverride)
                {
                    hyItemsDict.Remove(itemData.id);
                    hyItemsDict.Add(itemData.id, itemData);
                }
            }
            else
            {
                hyItemsDict.Add(itemData.id, itemData);
            }
        }
    }
}
