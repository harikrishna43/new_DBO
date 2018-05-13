using DBO.Data;
using DBO.Data.Models;

using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace DBO.App_Start
{
    public class ResourceConfig
    {
        private Language english;
        private Language danish;

        private List<Resource> resources;
        private DbSet<Resource> savedResources;
        private DbSet<Language> languages;
        private DbSet<Document> documents;

        public async Task PopulateResources()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                documents = context.Documents;
                languages = context.Languages;

                english = await AddLanguageIfNotExist("English", "en-us");
                danish = await AddLanguageIfNotExist("Danish", "da");
                await context.SaveChangesAsync();

                await AddDocumentIfNotExist("Intro email", "~/Templates/IntroEmail.html");
                await AddDocumentIfNotExist("Claim company", "~/Templates/ClaimCompany.html");
                await AddDocumentIfNotExist("Claim company referal", "~/Templates/ClaimCompanyReferal.html");

                //avoid roundtrips
                savedResources = context.Resources;
                resources = context.Resources.ToList();

                PopulateAllResources();
                await context.SaveChangesAsync();
            }
        }

        private async Task AddDocumentIfNotExist(string name, string filePath)
        {
            var document = await documents.FirstOrDefaultAsync(x => x.Name == name);
            if (document == null)
            {
                document = new Document
                {
                    Name = name,
                    Content = File.ReadAllText(HostingEnvironment.MapPath(filePath)),
                    LanguageId = english.Id
                };

                documents.Add(document);
            }
        }

        private async Task<Language> AddLanguageIfNotExist(string name, string cultureCode)
        {
            var language = await languages.FirstOrDefaultAsync(x => x.Name == name);
            if (language == null)
            {
                languages.Add(new Language
                {
                    Name = name,
                });
            }

            return language;
        }

        private void PopulateAllResources()
        {
            AddIfNotExist("ClaimedDetails_ButtonCaption", "", "{0} er mit firma - Tilmeld mig");
            AddIfNotExist("Address", "Address", "Adresse");
            AddIfNotExist("AdvancedSearch", "Advanced search", "Avanceret sogning");
            AddIfNotExist("AdvertisingProtection", "Advertising protection", "");
            AddIfNotExist("Agreement", "Agreement", "Medlemsbetingelserne");
            AddIfNotExist("AreYouSureToDelete", "Are you sure you want to delete this?", "Er du sikker på, at du vil slette dette?");
            AddIfNotExist("Back", "Back", "Tilbage");
            AddIfNotExist("BackToList", "Back to List", "Tilbage til liste");
            AddIfNotExist("Chairman", "Chairman", "Formand");
            AddIfNotExist("City", "City", "By");
            AddIfNotExist("Claimed", "Claimed", "Hævdede");
            AddIfNotExist("ClaimedDetails_BottomBtnCapture", "Submit information", "Indsend oplysninger");
            AddIfNotExist("ClaimedDetails_BottomCapture", "Thank you in advance", "På forhånd tak");
            AddIfNotExist("ClaimedDetails_BottomHeader", "Or can you help us get in touch with {0}?", "Eller kan du hjælpe os med at få kontakt til {0}?");
            AddIfNotExist("ClaimedDetails_FirstListItem", "Ability to create news", "Mulighed for at oprette nyheder");
            AddIfNotExist("ClaimedDetails_FourthListItem", "Bid for tenders/offers", "Byde ind på licitationer/udbud");
            AddIfNotExist("ClaimedDetails_RigthMenuMainText", "Membership at DBO is free and non-binding - so sign up today and enjoy the many member benefits:", "Medlemskab hos DBO er gratis og uforpligtende - så tilmeld dig allerede i dag og få glæde af de mange medlemsfordele:");
            AddIfNotExist("ClaimedDetails_SecondListItem", "Ability to write your own business history", "Mulighed for at skrive din egen virksomhedshistorie");
            AddIfNotExist("ClaimedDetails_ThirdListItem", "Connect with other companies", "Connecte med andre virksomheder");
            AddIfNotExist("ClaimedDetails_TopBody", "Take ownership of {0} and immediately get all the many benefits", "Tag ejerskab over {0} og opnå med det samme alle de mange fordele");
            AddIfNotExist("ClaimedDetails_TopHeader", "Is {0} your business?", "Er {0} din virksomhed?");
            AddIfNotExist("Claims", "Claims", "Claims");
            AddIfNotExist("Comment", "Comment", "Kommentar");
            AddIfNotExist("Comments", "comments", "kommentarer");
            AddIfNotExist("Companies", "Companies", "Virksomheder");
            AddIfNotExist("Company", "Company", "Selskab");
            AddIfNotExist("CompanyAlreadyExists", "Company already exists: {0} - {1}", "Company already exists: {0} - {1}");
            AddIfNotExist("CompanyName", "Company name", "Virksomhedsnavn");
            AddIfNotExist("ConfirmAgreement", "I have full permission to act on behalf of this company and confirm the terms of the", "Jeg har den fulde tilladelse til at agere på vegne af denne virksomhed, og bekræfter");
            AddIfNotExist("ConfirmPassword", "ConfirmPassword", "Confirm Password");
            AddIfNotExist("Connections", "Connections", "Tilslutninger");
            AddIfNotExist("ConnectMainInfo", "We lack information that lets you connect. If you have a name and email at a contact at {0}, please enter this", "Vi mangler oplysninger der gør at du kan connecte. Hvis du har navn og e-mail på en kontaktperson ved {0}, bedes du indtaste dette");
            AddIfNotExist("ConnectWith", "Connect with {0}", "Connet med {0}");
            AddIfNotExist("Contact", "Contact", "Kontaktperson");
            AddIfNotExist("ContactInfo", "Contact Info", "Contact Info");
            AddIfNotExist("Country", "Country", "Land");
            AddIfNotExist("Create", "Create", "Skab");
            AddIfNotExist("CreateNew", "Create New", "Lav ny");
            AddIfNotExist("CreateNewCompany", "Register your company", "Tilmeld din Virksomhed");
            AddIfNotExist("Delete", "Delete", "Slet");
            AddIfNotExist("Edit", "Edit", "Redigere");
            AddIfNotExist("Email", "Email", "Email");
            AddIfNotExist("EmailAdditionalInfo", "If you sign up with a different e-mail address than the one we have registered with the company, it requires manual authentication.", "Hvis du tilmelder dig med en anden e-mail adresse end den vi har registreret på virksomheden, kræver det en manuel godkendelse. ");
            AddIfNotExist("EmailReciveResponce", "You will receive a response to your registration within 24 hours.", "Du vil modtage svar på din tilmelding indenfor 24 timer.");
            AddIfNotExist("Employees", "Employees", "Employees");
            AddIfNotExist("EnterAdditionalEmail", "Enter your email address:", "Enter your email address:");
            AddIfNotExist("Found", "Found", "Fundet");
            AddIfNotExist("From", "From", "Fra");
            AddIfNotExist("Image", "Image", "Billede");
            AddIfNotExist("Industry", "Industry", "Industri");
            AddIfNotExist("IndustryCode", "Industry code", "Industri kode");
            AddIfNotExist("IndustryText", "Industry text", "Industri tekst");
            AddIfNotExist("InvalidLoginAttempt", "Invalid login attempt.", "Invalid login attempt.");
            AddIfNotExist("IsUnprocessed", "Is Unprocessed", "Is Unprocessed");
            AddIfNotExist("JobExchange", "Job exchange", "Opgavebørs");
            AddIfNotExist("LeaveAComment", "Leave a comment...", "Skriv kommentar...");
            AddIfNotExist("LegalInformations", "LegalInformations", "LegalInformations");
            AddIfNotExist("LoadMore", "Load more", "Indlæs mere");
            AddIfNotExist("LogIn", "Log in", "Log in");
            AddIfNotExist("Name", "Name", "Navn");
            AddIfNotExist("NewsFeeds", "NewsFeeds", "Neews feeds");
            AddIfNotExist("Number", "Number", "Nummer");
            AddIfNotExist("OnlyActiveMembers", "Only active members", "Kun aktive medlemmer");
            AddIfNotExist("OnlyNotActiveMembers", "Only not active members", "Kun ikke aktive medlemmer");
            AddIfNotExist("OnlyUnprocessed", "Only Unprocessed", "Only Unprocessed");
            AddIfNotExist("Owner", "Owner", "Ejer");
            AddIfNotExist("Password", "Password", "Password");
            AddIfNotExist("PersonName", "Person name", "Person navn");
            AddIfNotExist("Phone", "Phone", "Telefon");
            AddIfNotExist("PostCode", "Post code", "Postnummer");
            AddIfNotExist("ReadMoreLink", "Read more", "Læs mere");
            AddIfNotExist("Register", "Register", "Register");
            AddIfNotExist("RegisterClaimedCompany_MainTopText", "If you are the owner of {0} or otherwise affiliated with the company and are authorized to act on behalf of this, you can take ownership of the company:", "Hvis du er indehaver af {0} eller på anden måde tilknyttetvirksomheden, og har tilladelse til at agere på vegne af denne, kan du tageejerskab over virksomheden:");
            AddIfNotExist("Registrations", "Registrations", "Registrations");
            AddIfNotExist("RegistrationSuccessfulMessage", "Thank you for company claim request.", "Thank you for company claim request.");
            AddIfNotExist("Save", "Save", "Save");
            AddIfNotExist("Search", "Search", "Sog");
            AddIfNotExist("SearchCompanies", "Search companies", "Sog i virksomheder");
            AddIfNotExist("Select_industry", "Select industry", "Select industry");
            AddIfNotExist("SendIntroMail", "Send intro mail", "Send intro mail");
            AddIfNotExist("SendToYourPhone", "Send to your phone", "Send til din telefon");
            AddIfNotExist("ShareMail", "You will receive an email with login information at the following email address", "Du vil modtage en e-mail med login oplysninger på følgende e-mail adresse");
            AddIfNotExist("SignIn", "Sign in", "Log pa");
            AddIfNotExist("SignUp", "Sign up", "Tilmeld");
            AddIfNotExist("Skills", "Skills", "Skills");
            AddIfNotExist("TextDescription", "Text description", "Tekstbeskrivelse");
            AddIfNotExist("Title", "Title", "Titel");
            AddIfNotExist("To", "To", "Til");
            AddIfNotExist("Views", "views", "visninger");
            AddIfNotExist("Web", "Web", "Web");
            AddIfNotExist("AvailableSkills", "Number of available skills", "Antal tilgængelige færdigheder");
            AddIfNotExist("BasicData", "Basic data", "Bassic data");
            AddIfNotExist("SkillsLimitReached", "Skills limit reached!", "Skills limit reached!");
            AddIfNotExist("ClaimCompanyEmail", "Field is required.", "Field is required.");
            AddIfNotExist("ClaimCompanyName", "Field is required.", "Field is required.");
            AddIfNotExist("Send", "Send", "Sende");
            AddIfNotExist("WriteTo", "Write to", "Skriv til ");
            AddIfNotExist("WriteVisit", "Write your visit here...", "Skriv din besøg her");
            AddIfNotExist("SelectImage", "Select image", "Vælg billede");
            AddIfNotExist("Following", "Following", "Følge");
            AddIfNotExist("Follow", "Follow", "Følge efter");
            AddIfNotExist("ClaimedDetails_TopButtonCaption", "{0} is my company - Register me", "{0} er mit firma - Tilmeld mig");
            AddIfNotExist("ClaimCompany_UserHasCompany", "Email is already in use", "Email er allerede i brug");
            AddIfNotExist("AdvertisingProtecion", "Advertising protecion", "Reklamebeskyttelse");
            AddIfNotExist("StopFollowing", "Stop following", "Stop med at følge");
            AddIfNotExist("Connect", "Connect", "Forbinde");
            AddIfNotExist("Connected", "Connected", "Forbundet");
            AddIfNotExist("Disconnect", "Disconnect", "Koble fra");
            AddIfNotExist("WaitingForAccept", "Waiting for accept...", "Venter på at acceptere ...");
            AddIfNotExist("ScheduledEmails", "Scheduled emails", "Planlagte emails");
            AddIfNotExist("Translations", "Translations", "Oversættelser");
            AddIfNotExist("Invited", "Invited", "Inviteret");
            AddIfNotExist("InvitedNotRegistered", "Invited, not registered", "Inviteret, ikke registreret");
            AddIfNotExist("InvitedRegistered", "Invited, registered", "Inviteret, registreret");
            AddIfNotExist("SelfRegisteredCompanies", "Self-Registered companies", "Selvregistrerede virksomheder");
            AddIfNotExist("SelfRegisteredPersons", "Self-Registered persons", "Selvregistrerede personer");
            AddIfNotExist("Registered", "Registered", "Registreret");
            AddIfNotExist("NameInDatabase", "Name in database", "Navn i database");
            AddIfNotExist("NameInForm", "Name in form", "Navn i formular");
            AddIfNotExist("RequestTime", "Request time", "Anmodningstid");
            AddIfNotExist("ClaimStatus", "Claim status", "Claim status");
            AddIfNotExist("Subject", "Subject", "Emne");
            AddIfNotExist("Status", "Status", "Status");
            AddIfNotExist("CreatedAt", "Crated at", "Oprettet på");
            AddIfNotExist("UpdatedAt", "Updated at", "Opdateret kl");
            AddIfNotExist("Approve", "Approve", "Godkende");
            AddIfNotExist("Reject", "Reject", "Afvise");
            AddIfNotExist("English", "English", "Engelsk");
            AddIfNotExist("Danish", "Danish", "Dansk");
            AddIfNotExist("AddressNotProvided", "Address details not provided...", "Adresseoplysninger ikke angivet ...");
            AddIfNotExist("CreateNewEmployee", "Create new employee", "Opret ny medarbejder");
            AddIfNotExist("PurchaseMoreSkills", "Purchase more skills", "Køb flere færdigheder");
            AddIfNotExist("AddMore", "Add more", "Tilføj mere");
            AddIfNotExist("FollowingCompaniesWantsToConnect", "Following companies wants to connect:", "Følgende virksomheder ønsker at forbinde:");
            AddIfNotExist("Accept", "accept", "acceptere");
            AddIfNotExist("Decline", "decline", "nedgang");
            AddIfNotExist("LogOff", "Log off", "Log af");
            AddIfNotExist("Announce", "Announce", "Annoncere");
            AddIfNotExist("ValidateYourInput", "Please, validate your input.", "Vær venlig at validere din indtastning.");
            AddIfNotExist("SuccessfullySaved", "Successfully saved.", "Besvaret succesfuldt.");
            AddIfNotExist("Languages", "Languages", "Sprog");
            AddIfNotExist("Documents", "Documents", "Dokumenter");
            AddIfNotExist("Close", "Close", "Tæt");
            AddIfNotExist("AreYouSure", "Are you sure?", "Er du sikker?");
            AddIfNotExist("NotClaimed", "Not claimed", "Ikke påstået");
            AddIfNotExist("SignInAsThisCompany", "Sign in as this company", "Log ind som dette firma");
            AddIfNotExist("Skill", "Skill", "Dygtighed");
            AddIfNotExist("Details", "Details", "Detaljer");
            AddIfNotExist("Index", "Index", "Indeks");
            AddIfNotExist("ZipFrom", "Zip from", "Zip fra");
            AddIfNotExist("ZipTo", "Zip to", "Zip til");
            AddIfNotExist("IsProcessed", "Is processed", "Behandles");
            AddIfNotExist("Information", "Information", "Information");
            AddIfNotExist("Service", "Service", "Service");
            AddIfNotExist("About", "About", "Om");
            AddIfNotExist("TermsOfUse", "Terms of use", "Brugerbetingelser");
            AddIfNotExist("PrivacyPolicy", "Privacy policy", "Privatlivspolitik");
            AddIfNotExist("Advertising", "Advertising", "Annoncering");
            AddIfNotExist("Press", "Press", "Presse");
            AddIfNotExist("ReportAbuse", "Report abuse", "Anmeld misbrug");
            AddIfNotExist("ContactInformation", "Contact information", "Kontaktoplysninger");
            AddIfNotExist("BasicData", "Basic Data", "Basic Data");
            AddIfNotExist("Locations", "Locations", "Placeringer");
            AddIfNotExist("ShowAdFor", "Show ad for", "Vis annonce til");
            AddIfNotExist("LoggedIn", "Logged In", "Logget ind");
            AddIfNotExist("NotLoggedIn", "Not logged in", "Ikke logget ind");
            AddIfNotExist("PrivatePerson", "Private person", "privat person");
            AddIfNotExist("Content", "Content", "Indhold");
            AddIfNotExist("Headline", "Headline", "Overskrift");
            AddIfNotExist("Text", "Text", "Text");
            AddIfNotExist("ProfilePage", "Profile page", "Profil side");
            AddIfNotExist("ExternalLink", "External link", "Eksternt link");
            AddIfNotExist("Start", "Start", "Start");
            AddIfNotExist("End", "End", "Ende");
            AddIfNotExist("Immediately", "Immediately", "Med det samme");
            AddIfNotExist("BudgetLimit", "Budget limit", "Budgetgrænse");
            AddIfNotExist("TotalBudget", "Total budget", "Samlet budget");
            AddIfNotExist("PricePerClick", "Price per click", "Pris pr. Klik");
            AddIfNotExist("TargetGroup", "Target group", "Målgruppe");
            AddIfNotExist("FullWidth", "Full width", "Fuld bredde");
            AddIfNotExist("WithMargin", "With margin", "Med margin");
            AddIfNotExist("Upload", "Upload", "Upload");
            AddIfNotExist("Or", "Or", "Eller");
            AddIfNotExist("TimePeriod", "Time period", "Tidsperiode");
            AddIfNotExist("Payment", "Payment", "Betaling");
            AddIfNotExist("ContinueToPayment", "Continue to payment", "Fortsæt til betaling");
            AddIfNotExist("Remove", "Remove", "Fjerne");
            AddIfNotExist("Overview", "Overview", "Oversigt");
            AddIfNotExist("Link", "Link", "Link");
            AddIfNotExist("Manage", "Manage", "Styre");
            AddIfNotExist("Statistics", "Statistics", "Statistik");
            AddIfNotExist("Resume", "Resume", "Genoptag");
            AddIfNotExist("Stop", "Stop", "Hold op");
            AddIfNotExist("Pause", "Pause", "Pause");
            AddIfNotExist("RemainingBudget", "Remaining budget", "Resterende budget");
            AddIfNotExist("BudgetSpent", "Budget spent", "Budget brugt");
            AddIfNotExist("AdOpened", "Ad was opened", "Annoncen blev åbnet");
            AddIfNotExist("Layout", "Layout", "Layout");
            AddIfNotExist("Visibility", "Visibility", "Sigtbarhed");
            AddIfNotExist("ActiveFrom", "Active from", "Aktiv fra");
            AddIfNotExist("ActiveUntill", "Active until", "Aktiv indtil");
            AddIfNotExist("SelectAdvertisementType", "Select advertisement type", "Vælg annoncetype");
            AddIfNotExist("AddFile", "Add file", "Tilføj fil");
            AddIfNotExist("Active", "Active", "Aktiv");
            AddIfNotExist("Transfer", "Transfer", "Overfør");      
            AddIfNotExist("TransferBudget", "Transfer budget", "Overfør budget");
            AddIfNotExist("Advertisemet", "Advertisement", "Reklame");
            AddIfNotExist("For", "For", "Til");
            AddIfNotExist("Cancel", "Cancel", "Afbestille");
            AddIfNotExist("TransferToAnother", "Transfer budget to another advertisement", "Overfør budget til en anden annonce");
            AddIfNotExist("TransferToCurrent", "Transfer budget to current advertisement", "Overfør budget til nuværende annonce");
            AddIfNotExist("TransferBudgetError", "Set correct value for transfer budget", "Indstil korrekt værdi for overførselsbudget");
            AddIfNotExist("SelectAdvertisementBeforeTransfer", "Select advertisement before transfer", "Indstil korrekt værdi for overførselsbudget");
            AddIfNotExist("Select", "Select", "Vælg");
            AddIfNotExist("Amount", "Amount", "Beløb");
            AddIfNotExist("OnAPause", "On a pause", "På en pause");
            AddIfNotExist("Stopped", "Stopped", "Holdt op");
            AddIfNotExist("SuccessfullyTransferedBudget", "Successfully transfered budget", "Succesfuldt overført budget");
            AddIfNotExist("NotEnoughBudget", "Advertisement \"{ad}\" does not have enough of the current budget to transfer", "Annoncen \"{ad}\" har ikke nok af det nuværende budget til at overføre");
            AddIfNotExist("Video", "Video", "Video");
            AddIfNotExist("Cities", "Cities", "Byer");
            AddIfNotExist("WithoutLocation", "Without location", "Uden beliggenhed");
            AddIfNotExist("LocationType", "Location type", "Placeringstype");
            AddIfNotExist("ChooseCountry", "Choose country", "Vælg land");
            AddIfNotExist("ChooseCities", "Choose cities", "Vælg byer");
            AddIfNotExist("MaxCharacterCount", "Max {0} character", "Maks. {0} tegn");
            AddIfNotExist("CharactersLeft", "Characters left", "Tegn tilbage");
            AddIfNotExist("Notifications", "Notifications", "Underretninger");
            AddIfNotExist("NotifyOnConnectionRequest", "Notify me on connection request", "Underret mig om forbindelsesanmodning");
            AddIfNotExist("NotifyOnConnectionAccepts", "Notify me on connection accepts", "Underret mig om tilslutning accepterer");
            AddIfNotExist("NotifyMeOnNewFollower", "Notify me on new follower", "Underret mig om ny tilhænger");
            AddIfNotExist("NotifyOnesADay", "I want to be notified once a day", "Jeg vil gerne blive underrettet en gang om dagen");
            AddIfNotExist("NotifyOnesAWeek", "I want to be notified once a week", "Jeg vil gerne blive underrettet en gang om ugen");
            AddIfNotExist("DontWantToRecieveNotification", "I don't want to receive any notifications", "Jeg vil ikke modtage nogen underretninger");
            AddIfNotExist("ValidationErrorDefault", "This field is required", "Dette felt er påkrævet");
            AddIfNotExist("ValidationErrorEmail", "Please, enter correct email address", "Indtast venligst den rigtige email-adresse");
            AddIfNotExist("Browse", "Browse", "Gennemse");
            AddIfNotExist("CreateAnEmployee", "Create a \"{0}\" employee", "Opret en \"{0}\" medarbejder");
            AddIfNotExist("UpdateAnEmployee", "Update a \"{0}\" employee", "opdatere en \"{0}\" medarbejder");


        }
        private void AddIfNotExist(string name, string englishValue, string danishValue)
        {
            if (!resources.Any(x => x.Name == name && x.LanguageId == english.Id))
            {
                savedResources.Add(new Resource
                {
                    Name = name,
                    Value = englishValue,
                    LanguageId = english.Id
                });
            }

            if (!resources.Any(x => x.Name == name && x.LanguageId == danish.Id))
            {
                savedResources.Add(new Resource
                {
                    Name = name,
                    Value = danishValue,
                    LanguageId = danish.Id
                });
            }
        }
    }
}