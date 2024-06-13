using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class Constants
{
	public static class MessageToClient
	{

		public const string PHONE_NUMBER_NOT_EXISTS = "Phone number not exists";
		public const string PHONE_NUMBER_ALREADY_EXISTS = "Phone number already exists";
		public const string WRONG_OTP_ENTERED = "Wrong OTP Entered";
		public const string PLEASE_TRY_TO_REGISTER_AGAIN = "Please try to register again";
		public const string INVALID_EMAIL = "Invalid email";
		public const string INVALID_CREDENTAILS = "Invalid credentitals";
		public const string USER_EXISTS_TRY_TO_LOGIN = "User exists! Try to login";

	}

	public static class JwtToken
	{
		public static string ISSUER = Environment.GetEnvironmentVariable("ISSUER");
		public static string AUDIENCE = Environment.GetEnvironmentVariable("AUDIENCE");
		public const double EXPIRE_IN_DAYS = 365;
		public static string SECURITY_KEY_VALUE = Environment.GetEnvironmentVariable("SECURITY_KEY_VALUE");
		public static SymmetricSecurityKey SECURITY_KEY = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SECURITY_KEY_VALUE));
	}

	public static class Roles
	{
		public const string ADMIN = "admin";
		public const string USER = "user";


		public const string LOGIN = "login";
		public const string SIGN_UP = "signUp";
		public const string LOBBY = "lobby";
	}

	public static class GrainsReminderNames
	{
		public const string UNIQUESERVICE_UPDATE_LEADERBOARD_REMINDER = "UniqueService_UpdateLeaderboardReminder";
		public const string UNIQUESERVICE_UPDATE_USERID_TO_TOURNAMENT_PLACEID_REMINDER = "UniqueService_UpdateUserIdToTournamentPlaceIdReminder";
		public const string UNIQUESERVICE_LEADERBOARD_WEEKLY_CREDIT_WINNERS_BALANCES_REMINDER = "UniqueService_LeaderboardWeeklyCreditWinnersBalancesReminder";
		public const string UNIQUESERVICE_LEADERBOARD_WEEKLY_UPDATE_BOTS_STARS_REMINDER = "UniqueService_LeaderboardWeeklyUpdateBotsStarsReminder";
	}

	public static class RandomName
	{
		public static readonly string[] firstNames = {"attractive","bald","beautiful","chubby","clean","dazzling","drab","elegant","fancy","fit","flabby","glamorous","gorgeous","handsome","long","magnificent",
			"muscular","plain","plump","quaint","scruffy","shapely","short","skinny","stocky","ugly","unkempt","unsightly","alive","better","careful","clever","dead","easy","famous","gifted","hallowed","helpful",
			"important","inexpensive","mealy","mushy","odd","poor","powerful","rich","shy","tender","unimportant","uninterested","vast","wrong","aggressive","agreeable","ambitious","brave","calm","delightful",
			"eager","faithful","gentle","happy","jolly","kind","lively","nice","obedient","polite","proud","silly","thankful","victorious","witty","wonderful","zealous","angry","bewildered","clumsy","defeated",
			"embarrassed","fierce","grumpy","helpless","itchy","jealous","lazy","mysterious","nervous","obnoxious","panicky","pitiful","repulsive","scary","thoughtless","uptight","worried","big","colossal",
			"fat","gigantic","great","huge","immense","large","little","mammoth","massive","microscopic","miniature","petite","puny","scrawny","short","small","tall","teeny","tiny"};

		public static readonly string[] secondNames = { "Cow","Sheep","Rhinoceros","Raccoon","Pig","Panda","Chimpanzee","Bear","Bat","Zebra","Walrus","Tiger","Squirrel","Ox","Otter","Mouse","Monkey","Mole","Lion","Horse",
			"Hippopotamus","Hedgehog","Hamster","Fox","Elephant","Dog","Leopard","Koala","Kangaroo","Goat","Giraffe","Deer","Coyote","Dog","Mouse","Goldfish","Cat","Turtle","Guinea pig","Hamster","Parrot","Antelope",
			"Bald eagle","Porcupine","Bat","Arctic wolf","Badger","Bear","Camel","Gorilla","Hare","Hedgehog","Hippopotamus","Kangaroo","Koala","Chimpanzee","Coyote","Deer","Elephant","Elk","Fox","Giraffe","Leopard",
			"Lion","Lizard","Mole","Rabbit","Rhinoceros","Raccoon","Rat","Bison","Reindeer","Monkey","Otter","Owl","Panda","Zebra","Wombat","Red panda","Possum","Squirrel","Tiger","Walrus","Wolf","Woodpecker",
			"Chipmunk","Porcupine","Bee","Dog","Donkey","Dove","Cat","Chicken","Parrot","Pig","Rabbit","Sheep","Cow","Deer","Duck","Fish","Goat","Horse","Turkey","Deer","Fox","Tiger","Elephant","Lynxes",
			"Woodpecker","Orangutan","Howler monkey","Wild boar","Squirrel monkey","Hornbill","Great horned owl","Marten","African elephant","Red squirrel","Red dear","Okapi","Bengal tiger","Chimpanzee",
			"Gorilla","Raccoon","Badger","Rabbit","Hedgehog","Koala","Panda","Lion","Kangaroo","Coyote","Snake","Lizard","Antelope","Giraffe","Reindeer","Monkey","Hare","Puma","Hyena","Jackal","Rhinoceros",
			"Clam","Cormorant","Seal","Shark","Shell","Shrimp","Squid","Oyster","Pelican","Sea turtle","Starfish","Crab","Dolphin","Fish","Jellyfish","Lobster","Octopus","Otter","Seahorse","Walrus","Whale",
			"Parrot","Pelecaniformes","Turkey","Flamingo","Pigeon","Woodpecker","Cuckoo","Hawk","Bald eagle","Raven","Robin","Sparrow","Crow","Howler monkey","Swallow","Goose","Swan","Dove","Owl","Hummingbird",
			"Finch","Frigatebird","Sandpiper","Stork","Penguin","Tern","Ibis","Hornbill" };

		public static string GetRandomName()
		{
			Random rand = new Random();
			return firstNames[rand.Next(0, firstNames.Length)] + secondNames[rand.Next(0, secondNames.Length)];
		}
	}
}

