using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Drawing;

namespace ITR
{



    /// <summary>
    /// Exception thrown when ItemTextureResolver is being used without initialization
    /// </summary>
    public class ITRNotInitializedException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ITRNotInitializedException() : base() { }
    }

    /// <summary>
    /// All <a href="https://github.com/Bukkit/Bukkit/blob/master/src/main/java/org/bukkit/Material.java">Bukkit Materials</a>
    /// </summary>
#pragma warning disable CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
    public enum Material
    {
        AIR = 0,
        STONE = 1,
        GRASS = 2,
        DIRT = 3,
        COBBLESTONE = 4,
        WOOD = 5,
        SAPLING = 6,
        BEDROCK = 7,
        WATER = 8,
        STATIONARY_WATER = 9,
        LAVA = 10,
        STATIONARY_LAVA = 11,
        SAND = 12,
        GRAVEL = 13,
        GOLD_ORE = 14,
        IRON_ORE = 15,
        COAL_ORE = 16,
        LOG = 17,
        LEAVES = 18,
        SPONGE = 19,
        GLASS = 20,
        LAPIS_ORE = 21,
        LAPIS_BLOCK = 22,
        DISPENSER = 23,
        SANDSTONE = 24,
        NOTE_BLOCK = 25,
        BED_BLOCK = 26,
        POWERED_RAIL = 27,
        DETECTOR_RAIL = 28,
        PISTON_STICKY_BASE = 29,
        WEB = 30,
        LONG_GRASS = 31,
        DEAD_BUSH = 32,
        PISTON_BASE = 33,
        PISTON_EXTENSION = 34,
        WOOL = 35,
        PISTON_MOVING_PIECE = 36,
        YELLOW_FLOWER = 37,
        RED_ROSE = 38,
        BROWN_MUSHROOM = 39,
        RED_MUSHROOM = 40,
        GOLD_BLOCK = 41,
        IRON_BLOCK = 42,
        DOUBLE_STEP = 43,
        STEP = 44,
        BRICK = 45,
        TNT = 46,
        BOOKSHELF = 47,
        MOSSY_COBBLESTONE = 48,
        OBSIDIAN = 49,
        TORCH = 50,
        FIRE = 51,
        MOB_SPAWNER = 52,
        WOOD_STAIRS = 53,
        CHEST = 54,
        REDSTONE_WIRE = 55,
        DIAMOND_ORE = 56,
        DIAMOND_BLOCK = 57,
        WORKBENCH = 58,
        CROPS = 59,
        SOIL = 60,
        FURNACE = 61,
        BURNING_FURNACE = 62,
        SIGN_POST = 63,
        WOODEN_DOOR = 64,
        LADDER = 65,
        RAILS = 66,
        COBBLESTONE_STAIRS = 67,
        WALL_SIGN = 68,
        LEVER = 69,
        STONE_PLATE = 70,
        IRON_DOOR_BLOCK = 71,
        WOOD_PLATE = 72,
        REDSTONE_ORE = 73,
        GLOWING_REDSTONE_ORE = 74,
        REDSTONE_TORCH_OFF = 75,
        REDSTONE_TORCH_ON = 76,
        STONE_BUTTON = 77,
        SNOW = 78,
        ICE = 79,
        SNOW_BLOCK = 80,
        CACTUS = 81,
        CLAY = 82,
        SUGAR_CANE_BLOCK = 83,
        JUKEBOX = 84,
        FENCE = 85,
        PUMPKIN = 86,
        NETHERRACK = 87,
        SOUL_SAND = 88,
        GLOWSTONE = 89,
        PORTAL = 90,
        JACK_O_LANTERN = 91,
        CAKE_BLOCK = 92,
        DIODE_BLOCK_OFF = 93,
        DIODE_BLOCK_ON = 94,
        STAINED_GLASS = 95,
        TRAP_DOOR = 96,
        MONSTER_EGGS = 97,
        SMOOTH_BRICK = 98,
        HUGE_MUSHROOM_1 = 99,
        HUGE_MUSHROOM_2 = 100,
        IRON_FENCE = 101,
        THIN_GLASS = 102,
        MELON_BLOCK = 103,
        PUMPKIN_STEM = 104,
        MELON_STEM = 105,
        VINE = 106,
        FENCE_GATE = 107,
        BRICK_STAIRS = 108,
        SMOOTH_STAIRS = 109,
        MYCEL = 110,
        WATER_LILY = 111,
        NETHER_BRICK = 112,
        NETHER_FENCE = 113,
        NETHER_BRICK_STAIRS = 114,
        NETHER_WARTS = 115,
        ENCHANTMENT_TABLE = 116,
        BREWING_STAND = 117,
        CAULDRON = 118,
        ENDER_PORTAL = 119,
        ENDER_PORTAL_FRAME = 120,
        ENDER_STONE = 121,
        DRAGON_EGG = 122,
        REDSTONE_LAMP_OFF = 123,
        REDSTONE_LAMP_ON = 124,
        WOOD_DOUBLE_STEP = 125,
        WOOD_STEP = 126,
        COCOA = 127,
        SANDSTONE_STAIRS = 128,
        EMERALD_ORE = 129,
        ENDER_CHEST = 130,
        TRIPWIRE_HOOK = 131,
        TRIPWIRE = 132,
        EMERALD_BLOCK = 133,
        SPRUCE_WOOD_STAIRS = 134,
        BIRCH_WOOD_STAIRS = 135,
        JUNGLE_WOOD_STAIRS = 136,
        COMMAND = 137,
        BEACON = 138,
        COBBLE_WALL = 139,
        FLOWER_POT = 140,
        CARROT = 141,
        POTATO = 142,
        WOOD_BUTTON = 143,
        SKULL = 144,
        ANVIL = 145,
        TRAPPED_CHEST = 146,
        GOLD_PLATE = 147,
        IRON_PLATE = 148,
        REDSTONE_COMPARATOR_OFF = 149,
        REDSTONE_COMPARATOR_ON = 150,
        DAYLIGHT_DETECTOR = 151,
        REDSTONE_BLOCK = 152,
        QUARTZ_ORE = 153,
        HOPPER = 154,
        QUARTZ_BLOCK = 155,
        QUARTZ_STAIRS = 156,
        ACTIVATOR_RAIL = 157,
        DROPPER = 158,
        STAINED_CLAY = 159,
        STAINED_GLASS_PANE = 160,
        LEAVES_2 = 161,
        LOG_2 = 162,
        ACACIA_STAIRS = 163,
        DARK_OAK_STAIRS = 164,
        SLIME_BLOCK = 165,
        BARRIER = 166,
        IRON_TRAPDOOR = 167,
        PRISMARINE = 168,
        SEA_LANTERN = 169,
        HAY_BLOCK = 170,
        CARPET = 171,
        HARD_CLAY = 172,
        COAL_BLOCK = 173,
        PACKED_ICE = 174,
        DOUBLE_PLANT = 175,
        STANDING_BANNER = 176,
        WALL_BANNER = 177,
        DAYLIGHT_DETECTOR_INVERTED = 178,
        RED_SANDSTONE = 179,
        RED_SANDSTONE_STAIRS = 180,
        DOUBLE_STONE_SLAB2 = 181,
        STONE_SLAB2 = 182,
        SPRUCE_FENCE_GATE = 183,
        BIRCH_FENCE_GATE = 184,
        JUNGLE_FENCE_GATE = 185,
        DARK_OAK_FENCE_GATE = 186,
        ACACIA_FENCE_GATE = 187,
        SPRUCE_FENCE = 188,
        BIRCH_FENCE = 189,
        JUNGLE_FENCE = 190,
        DARK_OAK_FENCE = 191,
        ACACIA_FENCE = 192,
        SPRUCE_DOOR = 193,
        BIRCH_DOOR = 194,
        JUNGLE_DOOR = 195,
        ACACIA_DOOR = 196,
        DARK_OAK_DOOR = 197,
        END_ROD = 198,
        CHORUS_PLANT = 199,
        CHORUS_FLOWER = 200,
        PURPUR_BLOCK = 201,
        PURPUR_PILLAR = 202,
        PURPUR_STAIRS = 203,
        PURPUR_DOUBLE_SLAB = 204,
        PURPUR_SLAB = 205,
        END_BRICKS = 206,
        BEETROOT_BLOCK = 207,
        GRASS_PATH = 208,
        END_GATEWAY = 209,
        COMMAND_REPEATING = 210,
        COMMAND_CHAIN = 211,
        FROSTED_ICE = 212,
        MAGMA = 213,
        NETHER_WART_BLOCK = 214,
        RED_NETHER_BRICK = 215,
        BONE_BLOCK = 216,
        STRUCTURE_VOID = 217,
        OBSERVER = 218,
        WHITE_SHULKER_BOX = 219,
        ORANGE_SHULKER_BOX = 220,
        MAGENTA_SHULKER_BOX = 221,
        LIGHT_BLUE_SHULKER_BOX = 222,
        YELLOW_SHULKER_BOX = 223,
        LIME_SHULKER_BOX = 224,
        PINK_SHULKER_BOX = 225,
        GRAY_SHULKER_BOX = 226,
        SILVER_SHULKER_BOX = 227,
        CYAN_SHULKER_BOX = 228,
        PURPLE_SHULKER_BOX = 229,
        BLUE_SHULKER_BOX = 230,
        BROWN_SHULKER_BOX = 231,
        GREEN_SHULKER_BOX = 232,
        RED_SHULKER_BOX = 233,
        BLACK_SHULKER_BOX = 234,
        WHITE_GLAZED_TERRACOTTA = 235,
        ORANGE_GLAZED_TERRACOTTA = 236,
        MAGENTA_GLAZED_TERRACOTTA = 237,
        LIGHT_BLUE_GLAZED_TERRACOTTA = 238,
        YELLOW_GLAZED_TERRACOTTA = 239,
        LIME_GLAZED_TERRACOTTA = 240,
        PINK_GLAZED_TERRACOTTA = 241,
        GRAY_GLAZED_TERRACOTTA = 242,
        SILVER_GLAZED_TERRACOTTA = 243,
        CYAN_GLAZED_TERRACOTTA = 244,
        PURPLE_GLAZED_TERRACOTTA = 245,
        BLUE_GLAZED_TERRACOTTA = 246,
        BROWN_GLAZED_TERRACOTTA = 247,
        GREEN_GLAZED_TERRACOTTA = 248,
        RED_GLAZED_TERRACOTTA = 249,
        BLACK_GLAZED_TERRACOTTA = 250,
        CONCRETE = 251,
        CONCRETE_POWDER = 252,
        STRUCTURE_BLOCK = 255,    // ----- Item Separator -----

        IRON_SPADE = 256,
        IRON_PICKAXE = 257,
        IRON_AXE = 258,
        FLINT_AND_STEEL = 259,
        APPLE = 260,
        BOW = 261,
        ARROW = 262,
        COAL = 263,
        DIAMOND = 264,
        IRON_INGOT = 265,
        GOLD_INGOT = 266,
        IRON_SWORD = 267,
        WOOD_SWORD = 268,
        WOOD_SPADE = 269,
        WOOD_PICKAXE = 270,
        WOOD_AXE = 271,
        STONE_SWORD = 272,
        STONE_SPADE = 273,
        STONE_PICKAXE = 274,
        STONE_AXE = 275,
        DIAMOND_SWORD = 276,
        DIAMOND_SPADE = 277,
        DIAMOND_PICKAXE = 278,
        DIAMOND_AXE = 279,
        STICK = 280,
        BOWL = 281,
        MUSHROOM_SOUP = 282,
        GOLD_SWORD = 283,
        GOLD_SPADE = 284,
        GOLD_PICKAXE = 285,
        GOLD_AXE = 286,
        STRING = 287,
        FEATHER = 288,
        SULPHUR = 289,
        WOOD_HOE = 290,
        STONE_HOE = 291,
        IRON_HOE = 292,
        DIAMOND_HOE = 293,
        GOLD_HOE = 294,
        SEEDS = 295,
        WHEAT = 296,
        BREAD = 297,
        LEATHER_HELMET = 298,
        LEATHER_CHESTPLATE = 299,
        LEATHER_LEGGINGS = 300,
        LEATHER_BOOTS = 301,
        CHAINMAIL_HELMET = 302,
        CHAINMAIL_CHESTPLATE = 303,
        CHAINMAIL_LEGGINGS = 304,
        CHAINMAIL_BOOTS = 305,
        IRON_HELMET = 306,
        IRON_CHESTPLATE = 307,
        IRON_LEGGINGS = 308,
        IRON_BOOTS = 309,
        DIAMOND_HELMET = 310,
        DIAMOND_CHESTPLATE = 311,
        DIAMOND_LEGGINGS = 312,
        DIAMOND_BOOTS = 313,
        GOLD_HELMET = 314,
        GOLD_CHESTPLATE = 315,
        GOLD_LEGGINGS = 316,
        GOLD_BOOTS = 317,
        FLINT = 318,
        PORK = 319,
        GRILLED_PORK = 320,
        PAINTING = 321,
        GOLDEN_APPLE = 322,
        SIGN = 323,
        WOOD_DOOR = 324,
        BUCKET = 325,
        WATER_BUCKET = 326,
        LAVA_BUCKET = 327,
        MINECART = 328,
        SADDLE = 329,
        IRON_DOOR = 330,
        REDSTONE = 331,
        SNOW_BALL = 332,
        BOAT = 333,
        LEATHER = 334,
        MILK_BUCKET = 335,
        CLAY_BRICK = 336,
        CLAY_BALL = 337,
        SUGAR_CANE = 338,
        PAPER = 339,
        BOOK = 340,
        SLIME_BALL = 341,
        STORAGE_MINECART = 342,
        POWERED_MINECART = 343,
        EGG = 344,
        COMPASS = 345,
        FISHING_ROD = 346,
        WATCH = 347,
        GLOWSTONE_DUST = 348,
        RAW_FISH = 349,
        COOKED_FISH = 350,
        INK_SACK = 351,
        BONE = 352,
        SUGAR = 353,
        CAKE = 354,
        BED = 355,
        DIODE = 356,
        COOKIE = 357,    /**
     * @see org.bukkit.map.MapView
     */

        MAP = 358,
        SHEARS = 359,
        MELON = 360,
        PUMPKIN_SEEDS = 361,
        MELON_SEEDS = 362,
        RAW_BEEF = 363,
        COOKED_BEEF = 364,
        RAW_CHICKEN = 365,
        COOKED_CHICKEN = 366,
        ROTTEN_FLESH = 367,
        ENDER_PEARL = 368,
        BLAZE_ROD = 369,
        GHAST_TEAR = 370,
        GOLD_NUGGET = 371,
        NETHER_STALK = 372,
        POTION = 373,
        GLASS_BOTTLE = 374,
        SPIDER_EYE = 375,
        FERMENTED_SPIDER_EYE = 376,
        BLAZE_POWDER = 377,
        MAGMA_CREAM = 378,
        BREWING_STAND_ITEM = 379,
        CAULDRON_ITEM = 380,
        EYE_OF_ENDER = 381,
        SPECKLED_MELON = 382,
        MONSTER_EGG = 383,
        EXP_BOTTLE = 384,
        FIREBALL = 385,
        BOOK_AND_QUILL = 386,
        WRITTEN_BOOK = 387,
        EMERALD = 388,
        ITEM_FRAME = 389,
        FLOWER_POT_ITEM = 390,
        CARROT_ITEM = 391,
        POTATO_ITEM = 392,
        BAKED_POTATO = 393,
        POISONOUS_POTATO = 394,
        EMPTY_MAP = 395,
        GOLDEN_CARROT = 396,
        SKULL_ITEM = 397,
        CARROT_STICK = 398,
        NETHER_STAR = 399,
        PUMPKIN_PIE = 400,
        FIREWORK = 401,
        FIREWORK_CHARGE = 402,
        ENCHANTED_BOOK = 403,
        REDSTONE_COMPARATOR = 404,
        NETHER_BRICK_ITEM = 405,
        QUARTZ = 406,
        EXPLOSIVE_MINECART = 407,
        HOPPER_MINECART = 408,
        PRISMARINE_SHARD = 409,
        PRISMARINE_CRYSTALS = 410,
        RABBIT = 411,
        COOKED_RABBIT = 412,
        RABBIT_STEW = 413,
        RABBIT_FOOT = 414,
        RABBIT_HIDE = 415,
        ARMOR_STAND = 416,
        IRON_BARDING = 417,
        GOLD_BARDING = 418,
        DIAMOND_BARDING = 419,
        LEASH = 420,
        NAME_TAG = 421,
        COMMAND_MINECART = 422,
        MUTTON = 423,
        COOKED_MUTTON = 424,
        BANNER = 425,
        END_CRYSTAL = 426,
        SPRUCE_DOOR_ITEM = 427,
        BIRCH_DOOR_ITEM = 428,
        JUNGLE_DOOR_ITEM = 429,
        ACACIA_DOOR_ITEM = 430,
        DARK_OAK_DOOR_ITEM = 431,
        CHORUS_FRUIT = 432,
        CHORUS_FRUIT_POPPED = 433,
        BEETROOT = 434,
        BEETROOT_SEEDS = 435,
        BEETROOT_SOUP = 436,
        DRAGONS_BREATH = 437,
        SPLASH_POTION = 438,
        SPECTRAL_ARROW = 439,
        TIPPED_ARROW = 440,
        LINGERING_POTION = 441,
        SHIELD = 442,
        ELYTRA = 443,
        BOAT_SPRUCE = 444,
        BOAT_BIRCH = 445,
        BOAT_JUNGLE = 446,
        BOAT_ACACIA = 447,
        BOAT_DARK_OAK = 448,
        TOTEM = 449,
        SHULKER_SHELL = 450,
        IRON_NUGGET = 452,
        KNOWLEDGE_BOOK = 453,
        GOLD_RECORD = 2256,
        GREEN_RECORD = 2257,
        RECORD_3 = 2258,
        RECORD_4 = 2259,
        RECORD_5 = 2260,
        RECORD_6 = 2261,
        RECORD_7 = 2262,
        RECORD_8 = 2263,
        RECORD_9 = 2264,
        RECORD_10 = 2265,
        RECORD_11 = 2266,
        RECORD_12 = 2267
    }
#pragma warning restore CS1591 // Brak komentarza XML dla widocznego publicznie typu lub składowej
    /// <summary>
    /// This class contains most needed info about Item
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform(platformName: "windows")]
    public class Item
    {
        /// <summary>
        /// Item in-game name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// item ID used by HyPixel SkyBlock
        /// </summary>
        public string HyPixel_ID { get; }

        /// <summary>
        /// Bukkit Material of Item
        /// </summary>
        public Material Material { get; }

        /// <summary>
        /// Meta ID of Item
        /// </summary>
        public int Meta_ID { get; }

        /// <summary>
        /// GIF or PNG Image
        /// </summary>
        public Image Texture { get; }

        /// <summary>
        /// Is this texture of item original from HyPixel_ID? (or material texture)
        /// </summary>
        public bool IsOriginalTexture { get; }

        /// <summary>
        /// Has enchanted effect?
        /// </summary>
        public bool Glow { get; }

        /// <param name="name">Item in-game name</param>
        /// <param name="hyPixel_ID">item ID used by HyPixel SkyBlock</param>
        /// <param name="material">Bukkit Material of Item</param>
        /// <param name="originalTexture">Is this texture of item original from HyPixel_ID? (or material texture)</param>
        /// <param name="texture">GIF or PNG Image</param>
        /// <param name="glow">Has enchanted effect?</param>
        /// <param name="meta_ID">Meta ID</param>
        public Item(string name, string hyPixel_ID, Material material, bool originalTexture, Image texture, bool glow, int meta_ID = 0)
        {
            Name = name;
            HyPixel_ID = hyPixel_ID;
            Material = material;
            Meta_ID = meta_ID;
            IsOriginalTexture = originalTexture;
            Texture = texture;
            Glow = glow;
        }
    }

    /// <summary>
    /// ItemTextureResolver is class used to match Item to HyPixel_ID<br/>
    /// and show it's texture from vanilla or loaded Resourcepacks.<br/>
    /// Also capable to return Bukkit material texture<br/>if HyPixel_ID texture
    /// is not found.<br/><br/>
    /// Initialize with  <c>.Init()</c>  before using!!!
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform(platformName: "windows")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Style nazewnictwa", Justification = "Deserialization From JSON requires names to exact match")]
    public class ItemTextureResolver
    {
        private struct HyItems
        {

            public bool success { get; set; }
            public UInt64 lastUpdated { get; set; }
            public List<HyItems_Item> items { get; set; }
        }

        private struct HyItems_Item
        {
            public string id { get; set; }
            public string material { get; set; }
            public int durability { get; set; }
            public bool glowing { get; set; }
            public string name { get; set; }
            public string tier { get; set; }
            public string color { get; set; }
            public string skin { get; set; }
        }

        private struct Cit_Item
        {
            public string HyPixel_ID { get; set; }
            public string Name_pattern { get; set; }
            public Image Texture { get; set; }
        }

        private struct VanillaID
        {
            public int type { get; set; }
            public int meta { get; set; }
            public string name { get; set; }
            public string text_type { get; set; }
        }

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
            downloader = new(SkullDownloader);
            downloader.IsBackground = true;
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

        }

        void SkullDownloader()
        {
            while (!skullDownloadQueue.IsEmpty)
            {
                skullDownloadQueue.TryDequeue(out var item);
                Cit_Item cit = new();

                cit.HyPixel_ID = item.id;

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

                cit.Texture = Image.FromStream(responde.Result.Content.ReadAsStream());
                ((Bitmap)cit.Texture).MakeTransparent(Color.White);

                lock (citDict_Lock)
                {
                    citDict[Enum.Parse<Material>(item.material)].Add(cit);
                }

                var memStream = new MemoryStream(2022);
                cit.Texture.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
                CacheAppend($"Skins/[{item.material}]{cit.HyPixel_ID}.png", memStream.ToArray());
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

        private Span<byte> CacheRead(string entry)
        {
            using var cacheFileStream = new FileStream(pathToCacheFile, FileMode.Open);
            using ZipArchive cacheArchive = new(cacheFileStream, ZipArchiveMode.Read);

            var getEntry = cacheArchive.GetEntry(entry);
            if (getEntry == null) { return null; }
            var zipEntry = getEntry.Open();
            var ret = new Span<byte>(new byte[zipEntry.Length]);
            zipEntry.Seek(0, SeekOrigin.Begin);
            zipEntry.Read(ret);
            return ret;
        }

        /// <summary>
        /// Initialization before use of <c>ItemTextureResolver</c><br/>
        /// Items data is loaded from "ITR_Cache.zip", or if not found form HyPixel API<br/>
        /// also if "ITR_Cache.zip" is not found, ALL skullitems will be downloaded using mc-heads API<br/>
        /// </summary>
        /// <param name="pathToCacheFile">Relative path to ITR cache, where downoaded data is stored</param>
        /// <param name="forceRefresh">if true, Items data is always downloaded from HyPixel Api</param>
        /// <param name="noFileCache">if true, cache file is not created. Not recomended, API is slow</param>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        [Obsolete("Use FastInit instead. Downloading all skullitems is VERY slow")]
        public async Task Init(string pathToCacheFile = @".\ITR_Cache.zip", bool forceRefresh = false, bool noFileCache = false)
        {
            Dictionary<Material, List<Cit_Item>> skullsCits = new();
            skullsCits.TryAdd(Material.SKULL_ITEM, new());
            skullsCits.TryAdd(Material.GOLD_NUGGET, new()); // ask questions to VERY_OFFICIAL_YELLOW_ROCK
            bool useLocalCache = File.Exists(pathToCacheFile) && !forceRefresh;
            Stream cacheFileStream = null;
            ZipArchive cacheArchive = null;
            string text;

            HyItems_Item barrier = new();
            barrier.id = "BARRIER";
            barrier.material = "BARRIER";
            barrier.name = "Barrier Block";
            hyItemsDict.Add(barrier.id, barrier);

            //download or unzip items
            if (useLocalCache)
            {
                cacheFileStream = File.OpenRead(pathToCacheFile);
                cacheArchive = new(cacheFileStream, ZipArchiveMode.Read, true);

                var Entry = cacheArchive.GetEntry("Items.json");
                var ItemsEntry = Entry.Open();
                byte[] ItemsUTF8 = new byte[Entry.Length];
                ItemsEntry.Read(ItemsUTF8);
                ItemsEntry.Close();
                text = Encoding.Unicode.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Unicode, ItemsUTF8));
            }
            else
            {
                var responde = await http.GetAsync("https://api.hypixel.net/resources/skyblock/items");
                text = await responde.Content.ReadAsStringAsync();

                if (!noFileCache)
                {
                    cacheFileStream = new FileStream(pathToCacheFile, FileMode.Create);
                    cacheArchive = new(cacheFileStream, ZipArchiveMode.Update, true);
                    var ItemsEntry = cacheArchive.CreateEntry("Items.json").Open();
                    var ItemsUTF8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(text));
                    ItemsEntry.Write(ItemsUTF8);
                    ItemsEntry.Flush();
                    ItemsEntry.Close();
                }
            }


            //deserialize Items
            var hyItems_JSON = JsonSerializer.Deserialize<HyItems>(text);
            foreach (var item in hyItems_JSON.items)
            {
                hyItemsDict.Add(item.id, item);
            }

            //download or unzip skull items
            if (useLocalCache)
            {
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
                        cit.Texture = Image.FromStream(img);
                    }

                    skullsCits[Enum.Parse<Material>(material)].Add(cit);
                }
            }
            else
            {
                async Task GetSkullTexture(HyItems_Item item)
                {
                    Cit_Item cit = new();

                    cit.HyPixel_ID = item.id;

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

                    var responde = await http.GetAsync($"https://mc-heads.net/head/{skin}/64");

                    cit.Texture = Image.FromStream(responde.Content.ReadAsStream());
                    ((Bitmap)cit.Texture).MakeTransparent(Color.White);


                    skullsCits[Enum.Parse<Material>(item.material)].Add(cit);
                }

                foreach (var item in hyItemsDict.Values)
                {
                    if (item.skin == null) continue;
                    //zapytasz czemu. Odopwiem niewiem. Albo i wiem. No dobra API throttluje i nie ma senzu rżnąć kilkaset na raz. 
                    await GetSkullTexture(item);
                }

                if (!noFileCache)
                {
                    foreach (var citCat in skullsCits)
                    {
                        foreach (var cit in citCat.Value)
                        {
                            var ItemsEntry = cacheArchive.CreateEntry($"Skins/[{Enum.GetName<Material>(citCat.Key)}]{cit.HyPixel_ID}.png").Open();
                            cit.Texture.Save(ItemsEntry, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
            }

            ZipArchive vanillaTex = new(new MemoryStream(Properties.Resources.itemsZIP));
            foreach (var item in itemsIDTable)
            {
                Cit_Item cit = new();
                cit.HyPixel_ID = ((Material)item.type).ToString();
                if (item.meta != 0)
                    cit.HyPixel_ID += ":" + item.meta;

                cit.Texture = Image.FromStream(vanillaTex.GetEntry($"{item.type}-{item.meta}.png").Open());
                citDict[(Material)item.type].Add(cit);
            }

            foreach (var cit in skullsCits)
            {
                citDict[cit.Key].AddRange(cit.Value);
            }

            if (!noFileCache)
            {
                cacheArchive.Dispose();
                cacheFileStream.Flush();
                cacheFileStream.Close();
            }
            _initialized = true;
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

            HyItems_Item barrier = new();
            barrier.id = "BARRIER";
            barrier.material = "BARRIER";
            barrier.name = "Barrier Block";
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
                        cit.Texture = Image.FromStream(img);
                    }

                    citDict[Enum.Parse<Material>(material)].Add(cit);
                }
            }

            ZipArchive vanillaTex = new(new MemoryStream(Properties.Resources.itemsZIP));
            foreach (var item in itemsIDTable)
            {
                Cit_Item cit = new();
                cit.HyPixel_ID = ((Material)item.type).ToString();
                if (item.meta != 0)
                    cit.HyPixel_ID += ":" + item.meta;

                cit.Texture = Image.FromStream(vanillaTex.GetEntry($"{item.type}-{item.meta}.png").Open());
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

            foreach (var entry in zip.Entries)
            {
                if (!entry.Name.Contains(".properties")) continue;
                var path = entry.FullName[..(entry.FullName.Length - entry.Name.Length)];
                var propFile = new StreamReader(entry.Open());

                string line;
                Cit_Item cit_Item = new();
                Material material = Material.AIR;
                string texturePath = string.Empty;

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

                cit_Item.Texture = Image.FromStream(zip.GetEntry(texturePath).Open());

                if (texturePath.Contains(".png", StringComparison.CurrentCultureIgnoreCase) && zip.GetEntry(texturePath + ".mcmeta") != null)
                {
                    var animMcMeta = JsonDocument.Parse(zip.GetEntry(texturePath + ".mcmeta").Open());
                    var frametime = animMcMeta.RootElement.GetProperty("animation").GetProperty("frametime").GetInt32() * 50;
                    bool verticalGif;
                    int frameResolution;
                    if (cit_Item.Texture.Size.Width > cit_Item.Texture.Size.Height)
                    {
                        verticalGif = false;
                        frameResolution = cit_Item.Texture.Size.Height;
                    }
                    else
                    {
                        verticalGif = true;
                        frameResolution = cit_Item.Texture.Size.Width;
                    }
                    
                    var gifMakeItHappener = new ImageProcessor.Imaging.Formats.GifEncoder(frameResolution,frameResolution);

                    int startingPoint = 0;
                    while(startingPoint<(verticalGif? cit_Item.Texture.Size.Height : cit_Item.Texture.Size.Width))
                    {
                        var factory = new ImageProcessor.ImageFactory();
                        factory.Load(cit_Item.Texture);
                        factory.Crop(new Rectangle(
                            verticalGif ? 0 : startingPoint,
                            verticalGif ? startingPoint : 0,
                            frameResolution,
                            frameResolution));
                        MemoryStream memS = new(69);
                        factory.Save(memS);
                        var frame = new ImageProcessor.Imaging.Formats.GifFrame();
                        frame.Delay = new TimeSpan(0,0,0,0,frametime);
                        frame.Image = Image.FromStream(memS);
                        gifMakeItHappener.AddFrame(frame);
                        startingPoint += frameResolution;
                    }
                    cit_Item.Texture = gifMakeItHappener.Save();

                }

                citDict[material].Add(cit_Item);
                loaded++;
            }

            return loaded;
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
                if (hyItemsDict.TryGetValue(hyPixel_ID, out HyItems_Item value))
                {
                    bool isOriginalTexture = true;
                    Item retItem;

                    Material material = Enum.Parse<Material>(value.material);
                    //Dark Oak Wood

                    HyItems_Item citTestTestant = value;
                    Predicate<Cit_Item> citTest = new(x => x.HyPixel_ID == citTestTestant.id || (x.Name_pattern != null && citTestTestant.name.Contains(x.Name_pattern, StringComparison.InvariantCultureIgnoreCase)));

                    int citIdx = citDict[material].FindIndex(citTest);
                    if (citIdx == -1)
                    {
                        if (material == Material.SKULL_ITEM && value.durability == 3)
                        {
                            citTestTestant.id = value.material + (value.durability != 0 ? ":" + value.durability.ToString() : "");
                            citIdx = citIdx = citDict[material].FindIndex(citTest);
                            var isAlready = false;
                            foreach (var item in skullDownloadQueue.ToArray())
                            {
                                if (item.id == hyPixel_ID) { isAlready = true; break; }
                            }

                            if (!isAlready) skullDownloadQueue.Enqueue(hyItemsDict[hyPixel_ID]);
                            if (!downloader.IsAlive)
                            {
                                downloader = new(SkullDownloader);
                                downloader.IsBackground = true;
                                downloader.Start();
                            }

                        }
                        else
                        {
                            citTestTestant.id = value.material + (value.durability != 0 ? ":" + value.durability.ToString() : "");
                            citIdx = citIdx = citDict[material].FindIndex(citTest);
                            if (citIdx == -1) citIdx = 0;
                        }
                        isOriginalTexture = false;
                    }

                    var toRet = citDict[material][citIdx];
                    var texture = (Image)citDict[material][citIdx].Texture.Clone();
                    if (value.color != null && !isOriginalTexture)
                    {
                        var colorString = value.color.Split(',');
                        Color color = Color.FromArgb(Convert.ToInt32(colorString[0]), Convert.ToInt32(colorString[1]), Convert.ToInt32(colorString[2]));
                        using ImageProcessor.ImageFactory imageFactory = new();
                        imageFactory.Load(texture);
                        imageFactory.Format(new ImageProcessor.Imaging.Formats.PngFormat());
                        imageFactory.ReplaceColor(Color.FromArgb(198, 92, 53), color, 64);
                        imageFactory.Brightness(-13);
                        MemoryStream memS = new(420);
                        imageFactory.Save(memS);
                        texture = Image.FromStream(memS);

                        //Bitmap bitmap = new(retItem.Texture);
                        //var color = value.color.Split(',');
                        //Color color1 = Color.FromArgb(196, Convert.ToInt32(color[0]), Convert.ToInt32(color[1]), Convert.ToInt32(color[2]));

                        //using (Graphics graphics = Graphics.FromImage(bitmap))
                        //{
                        //    graphics.FillRectangle(new SolidBrush(color1), new Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height));
                        //}
                        //bitmap.MakeTransparent(bitmap.GetPixel(0,0));
                        //retItem = new(value.name, value.id, material, isOriginalTexture, bitmap, hyItemsDict[hyPixel_ID].glowing, value.durability);
                    }
                    retItem = new(value.name, value.id, material, isOriginalTexture, texture, hyItemsDict[hyPixel_ID].glowing, value.durability);
                    return retItem;
                }
                if (!Initialized)
                {
                    throw (new ITRNotInitializedException());
                }
                return null;
            }
        }
    }
}
