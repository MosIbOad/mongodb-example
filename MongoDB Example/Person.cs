using MongoDB.Driver;
using MongoDB_Example;

const string connectionString = "mongodb://localhost:27017";
const string databaseName = "simple_db";
const string collectionName = "people";


static IMongoCollection<T> ConnectMongo<T>(in string collection)
{
	var client = new MongoClient(connectionString);
	var db = client.GetDatabase(databaseName);
	return db.GetCollection<T>(collection);
}

static async Task<List<PersonModel>> GetAllPersons()
{
	var usersTable = ConnectMongo<PersonModel>(collectionName);
	var results = await usersTable.FindAsync(x => true);
	return results.ToList();
}

static async Task<PersonModel> GetUserById(string id)
{
	var personTable = ConnectMongo<PersonModel>(collectionName);

	var getPerson = await personTable.FindAsync(x => true);

	var personList = getPerson.ToList();

	var person = personList.Find(x => x.Id == id);
	//Console.WriteLine(text);

	return person;
}

static async Task CreatePerson(PersonModel person)
{
	try
	{
		var personTable = ConnectMongo<PersonModel>(collectionName);
		await personTable.InsertOneAsync(person);
	}
	catch (Exception ex)
	{
		Console.WriteLine("[EXCEPTION CreatePerson]: " + ex.Message);
		Console.WriteLine("[EXCEPTION CreatePerson]: " + ex.StackTrace);
	}
}

static async Task UpdatePerson(PersonModel person)
{
	var personTable = ConnectMongo<PersonModel>(collectionName);
	var filter = Builders<PersonModel>.Filter.Eq("Id", person.Id);
	await personTable.ReplaceOneAsync(filter, person, new ReplaceOptions { IsUpsert = true });
}

static async Task RemovePerson(PersonModel person)
{
	var personTable = ConnectMongo<PersonModel>(collectionName);
	await personTable.DeleteOneAsync(x => x.Id == person.Id);
}



var user = await GetUserById("6500637bac61996eec2a08a0");

if(user != null)
{
	user.FirstName = "Replaced";
	await UpdatePerson(user);
	Console.WriteLine($"{user.FirstName} isimli kullanıcı değişti! ID: {user.Id}");
}
/*
string text = user == null ? "Belirtilen kişi bulunamadı!" : $"Kişi bulundu! Adı: {user.FirstName}";
Console.WriteLine(text);
*/


//await CreatePerson(new PersonModel { FirstName = "Test", LastName = "Testtt" });