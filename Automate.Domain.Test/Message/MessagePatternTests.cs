using Automate.Domain.MessageAnalysis;

namespace Automate.Domain.Test.Message;

public class MessagePatternTests
{
    #region Message Contents
    [
        Theory,
    #region Above
        //InlineData("Hey I just signed up for pest control with you guys but I need to cancel it.", false),
        //InlineData("Hi! I had signed up for an annual service last year and want to cancel it before it renews for another year.The service was great - I just have a baby on the way and need to save the money this year.Thank you! ", false),
        //InlineData("Looking for a quote for pest control in Hillsboro nh.  Specifically ants and bees.  ", true),
        //InlineData(@"Requesting in-home service at 111 Charlton Drive, Hampton, VA, 23666 Nostrum laborum volu", true),
        //InlineData("Hi Kevin, thank you for your follow up.Unfortunately, I needed to be at work by 12:30 and could not stay. Can I reschedule for Monday, June 3rd after 3pm works best?", false),
        //InlineData("I am trying to get an idea how much it cost to treat for bed bugs for my daughter she lives between Smithfield and Ivor and she is getting bitten regularly so far it seems to be just her bedroom   How expensive is it??", true),
        //InlineData("Carpenter and smaller ants", true),
        //InlineData("Kevin did you get the pictures I sent you", false),
        //InlineData("I have an account at 31 N Hill Drive and I noticed we have bumble bees near the roof in one area. Do you take care of that? ", false),
        //InlineData(@"Hello, We are interested in a quote for mosquito control service.Address is 6438 County Line Rd 14519. Thank you", true),
        //InlineData("I need someone to come out to the house tomorrow. Is there a way I can set that up online?", false),
        //InlineData(@"Hello? I have a technician coming today but since it's pouring out I would like to cancel today's appointment and reschedule it for another day. Otherwise, the chemicals the technician is using will be a waste as it will be washed away.Please confirm this message and reschedule my appointment.thank you.", false),
        //InlineData("Need to have property at 4118 Barnes St. Treated for roaches and fleas", true),
        //InlineData("My rental contract requires flea and tick treatment to the house upon exit. I need to schedule an inspection to determine if treatment is needed.", true),
        //InlineData("We would like to get service established.Thank you.", true),
        //InlineData("I have sold my house at 33 Boulder Creek Drive, Rush NY 14543 and I need to terminate my contract with Fox immediately please.I will recommend your services to the new owners.Kurt Keller", false),
        //InlineData("Had some ants recently so shopping around for pest control services", true),
        //InlineData("I called the number back you called from and talked to somebody there who is going to terminate the service.Thank you", false),
        //InlineData(@"Hey Team, I recently signed up for your services.I have a well 24 feet from the front of my house.What are the set back distances for the insecticides you use? Thanks! Mike", false),
        //InlineData("Hello!  I am with the Town of Emmitsburg.You have a solicitor in Town that is going door to door.  They do not have the appropriate permits.They must cease now.", false),
        //InlineData("I have some small ants in our bathrooms. Also, everyone in a while we get hornets but we can't find their nests. Can someone come our to treat?", true),
        //InlineData("Looking for an estimate for mice", true),
        //InlineData("Hello! Trying to get a quote on an indoor spray targeting spiders specifically. We’ve found quite a few brown recluses inside and not quite sure where they’re coming from", true),
        //InlineData("Want to get a quote to have home treated for a termite problem.", true),
        //InlineData("Pest control in Youngsville residential home", true),
        //InlineData("I have carpenter ants in a tree in my back yard.I was curious if it is worth trying to treat it and remove the ants or just cut the tree down? I was unsure of the success rate of getting rid of carpenter ants in a tree?", true),
        //InlineData("While HVAC was checking out an issue in attic, it found wasps / hornets nest (active). Can someone come out to spray? ", true),
        //InlineData("Hi, I was wondering how much it would cost to get rid of bed bugs ? ", true),
        //InlineData("We would like to schedule a free inspection and develop a treatment plan for our house. ", true),
        //InlineData("Need service for issue with ground bees and little moths in house", true),
        //InlineData("I have recurring payments for Fox services that I would like to cancel.", false),
        //InlineData("Good morning, I need someone to come out to my property to set up more traps inside and outside.My son saw a rat last night and there is a huge hole burrowed by the front door. If poison is an option I'd prefer that also. ", false),
        //InlineData("hi we have tiny red ants on our patio/porch @ 400 Eastfield Drive in Fairfield", true),
        //InlineData("We have a bee issue on our roof deck in Stonington.Can you help us?", true),
        //InlineData("We have ants everywhere on our main floor. Can you please come back and spray again since it rained right after the last time you came? Thank you!", false),
        //InlineData("Hi there. We are having an issue with gnats/fruit flies in our basement for the second straight year and we would like to know if you handle those?", true),
        //InlineData("Good morning! When is my next quarterly service spray? If it's not for a while, then I'd like to schedule an in between spray for interior and exterior.We are seeing too many spiders for my liking! (806 Braxton, Youngsville)", false),
        //InlineData("Consultation", true),
        //InlineData("I have a honey bee hive on the outside of my chimney that has been there for a few weeks", false),
        //InlineData("I googled that and you came up, do you know any?", false),
        //InlineData("I believe I have carpenter bees in the siding near my roof on side of house.Looking for an inspection and then if it is confirmed I’d like to proceed.Thank you.", true),
        //InlineData("Hi.I’ve used you guys in the past and I was wanting to start again.Last summer we had a horrible hornet problem", false),
        //InlineData("Can you call me back I have another question", false),
        //InlineData("How soon is a tech able to get out and deal with ant problem in my kitchen", true),
        //InlineData("I was hoping to get a quote for a one time spray for carpenter ants", true),
        //InlineData("I’ve been waiting for service all day.I called at 4pm and was told I was on the schedule for today.It’s 6pm now am I getting service today? This is crazy that I’ve waited all day.", false),
        //InlineData("Hi.Our next quarterly service is June 25, but we have seen 2 ants in the house over the past 3 days.Can we have a technician sent out tomorrow?", false),
        //InlineData("Help! I have a big ant problem!", true),
        //InlineData("Would like to get a quote for wasps control", true),
        //InlineData("I live in Pearl River Zip code 10965 would like a quote on treatment for Carpenter Ants approx 1900 square feet free standing house", true),
        //InlineData(@"Buenos días Busco trabajo ", false),
        //InlineData("ants and spiders in basement", true),
        //InlineData("What are your rates for pest u", true),
        //InlineData("Spray for bees and wasp", true),
        //InlineData("Hey Guys, I’m emailing to provide written notification of cancellation. The service has been good and I don’t have any complaints, but at this time I do not wish to continue services due to other financial commitments. I will definitely reach out in the future if I need you guys again.Please cancel my account and provide me with an email notification that it has been done. I don’t wish to talk to an “account representative” as I have read the agreement and know that I do not have to do anything more than provide written notification.", false),
        //InlineData("I live at 1 Maura Way and have ants, look like carpenter ants, need exterminator to control them", true),
        //InlineData("Looking for help with ants in my house and bugs all outside as soon as possible", true),
        //InlineData("Following up on an email I sent Thursday requesting targeted service for ants and mice in my home. Can I get someone out asap?", false),
        //InlineData("Good day, I am looking for a quote via text for a basic pest prevention with possible yearly contract.  No pests currently, just preventive maintenance.", true),
        //InlineData("I spoke with another person and   visit is planned for this coming Tuesday.", false),
        //InlineData("How to I schedule pest control for ants", true),
        //InlineData("Thanks for contacting Fox Pest Control. Our team will respond shortly.", false),
        //InlineData("Look I do want to go with yall cause my Neighbor use yall but I already paid for there services today...and my back yard has major fleas", false),
        //InlineData(@"Can you tell me a little about the product you use? I have dogs, will it be an issue for them? Is it sprayed monthly?", true),
        //InlineData("Hello would like an estimate please in bay shore", true),
        //InlineData("I want to set up a time for fox to come out , you guys came out 2.5 weeks ago and we are still infested with red bugs outside in our backyard, I would like to set up a time asap if possible", false),
        //InlineData("i still have tiny ants in my kitchen after last visit tried to address them", false),
        //InlineData("I have a bee problem I believe they have created a bee nest in my wall they are entering through the roof ", true),
        //InlineData(@"Hello, I have an account with Fox Pest Control. We have been seeing ants in our kitchen, on the floor and counters for the last few weeks. We had ants on the floor in the kitchen last year - I think they are coming from the doors that go out to the back deck. Would someone be able to take a look? Thank you, Rebekah Eyre", false),
        //InlineData("Hello I need to schedule a service I am already a customer", false),
        //InlineData("Hello - I am looking for a quote to have ground wasps and ear wigs treated at my residential property.", true),
        //InlineData(@"Liked “Hi Patrick, the email has been sent. I will close…”", false),
        //InlineData(@"Hello! I recently found quite a bit of mouse droppings in a passageway in my basement, Is it possible for someone to treat?", true),
        //InlineData(@"Stayed on hold for 18 minutes and don't have that kind of time", false),
        //InlineData("I just returned from vacation with pests all through my basement look like larvae can i get a same day service today", true),
        //InlineData("Hello, my sister recently used your services and has referred you.  I'm located in Stamford and would like to set up an appointment for an estimate.", false),
        //InlineData("I need to speak with someone about possibly canceling our contract.", false),
        //InlineData(@"Hi, I have bees (carpenter I think) in my attic that I need to get taken care of and am also interested in pest control for the exterior perimeter of my house expecially something that covers eves (2 story house) as that is where the bees seem to have been going in. Thanks, Matt", true),
        //InlineData("Hello I know we’re approaching the end of my contract I’d like to cancel it if possible once the contract ends thank you", false),
        //InlineData("When is technician coming today?", false),
        //InlineData("", true),
        //InlineData(" ", true),
        //InlineData("I would like to order the free termite inspection", true),
        //InlineData("I have my yard full of ants", true),
        //InlineData("Hi, I have a contract with you  113 Newton Rd Plaistow NH     We need someone for ants getting in the house. Your phones are all going to Xfinity", false),
        //InlineData("I have been on hold for 10 minutes 3 times today and cannot get anyone to pick up the phone.I have mouse poop in my silverware drawer for the 3rd time now.Every time you have come, I'm told it is resolved but clearly it is not. Can someone please call me back?!", false),
        //InlineData("I am an existing customer and need to schedule an appointment for ants inside the home. Had the same problem last year. ", false),
        //InlineData("Yes I was just curious when my scheduled service for today, 6/3/2024 was going to be completed.Haven't seen anything in my account showing the service was completed yet", false),
        //InlineData(@"My house is 1530 sq feet, normal yard I’d be a new customer and saw your ad for 50 dollars. How much for just the house? For house and yard? Thanks ", true),
        //InlineData("Bees inside house.Need removal.", true),
        //InlineData("My husband signed us up for the home protection services in Stow MA recently.We have found 3 wasp nests. Is this included in the services? ", false),
        //InlineData("I'm looking to get mice removed.  ", true),
        //InlineData("Good morning, I have a ton of black ants all over my fire pit area.Is there anything we can do about this?", true),
        //InlineData("Why was I charge $210.59 yesterday?", false),
        //InlineData("Hey. We still have some ants going into the house.We also have a new nest on the front porch, I don’t recognise which insect? I can send a picture", false),
        //InlineData(" Is this for the termite prevention?", true),
        //InlineData("Trying to get a time for my appointment today", false),
        //InlineData("We are current customers and having an ant problem in the back yard.", false),
        //InlineData("I am an existing customer and had a recent visit.  This morning we have some sort of bug that has visited our garage.It is a smaller very thin insect that flies and crawls.  (have picture I can send).  I've tried to open garage to see if they'll exit and have swept them out.  Didn't see them last night or earlier, but went out to dry clothes and there were a number of them.  I can't find the source.  ", false),
        //InlineData("Preparation list", false),
        //InlineData("Hi, I had my house serviced last week but I'm still noticing quite a bit of carpenter bees. I emailed two days ago too but wondering how long they will be around for ", false),
        //InlineData("Looking for tick treatment for my lawn.", true),
        //InlineData("I have mice in a new home I'm purchaing", true),
        //InlineData("Would like to schedule interior and exterior service for ant problem - I’m looking for anything Friday please ", true),
        //InlineData("Hey - i was just wondering when my next appointment was ", false),
        //InlineData(@"Fox Pest Control, We used you in the past and we were not happy with your service, you never got rid of the tiny roach-like creatures we had, we didn't ourselves with a simple over-the-counter gel.  It took months to cancel you, you kept trying to take my money and kept coming out to wipe my screens for 5 minutes when the problem was inside. Your sales people keep coming around to our house, even today when we clearly have a ""No Solicitation"" sign by our front door.Either teach your sales people to listen to the signs or explain what it means to them.We work from home most days and do not want your interruption into our meeting nor the noise you cause with our dogs barking at the doorbell when we told you not to. Please remove 2556 Elson Green Avenue from your list of houses to visit. Thank you, Douglas Starke", false),
        //InlineData("Thank you.  My wife is also recovering from knee surgery and had to leave her meeting to answer the door because the dogs wouldn't stop barking at him standing there.  All along with a sign staring at him.", false),
        //InlineData("Requesting for the bed bug treatment", true),
        //InlineData("No one answers at this number", false),
        //InlineData("I spoke with Jacob (?) earlier and sent second message.  He believes we have swarmer termites.  They were just in garage, but have slowly migrated into the house.  I'm spending an abundance of time wiping the floor to get rid of them.  Wondered if there was a temporary solution before someone checks tomorrow (scheduled).  I have a pet inside and have cloroxed and used other cleaners to try to bat them down.  Appreciate any advice.", false),
        //InlineData("I need to find out about canceling my service", false),
        //InlineData("I think I may have a flee infestation", true),
        //InlineData("Hi- someone is supposed to be coming to my house today to take care of some ants but I haven’t heard from anyone ", false),
        //InlineData("Hi, I need to get a tech out to deal with some ants. ", false),
        //InlineData("I would like to get a quote about your bug control treatment. My email is benbracy77 @gmail.com", true),
        //InlineData("Would like inspection of home for carpenter bees/wasps etc...and any damage that may have been done to the exterior of the home", true),
        //InlineData("Swarming ants or termites filed through my laundry. I am requesting an inspection and quote.Thank you.", true),
        //InlineData("Found a bee hive off my front porch would like to get it removed", true),
        //InlineData("Hey I was wondering if you do mosquito/yard treatments? I think my neighbors on both side use your service for general pest prevention.We are at 609 Thomas ave in Riverton", false),
        //InlineData("Would like to inquire about same day pest services. Haven't had my home treated in years so I'd like the inside done but also I'm having an infestation issue with these ants. I say ants because they're small, fast, and oddly enough they don't sting. There are thousands of these on my driveway and in and out of my garage. Please help! Lol", true),
        //InlineData("This morning I saw a lot of ants in my garage.  Can someone come out today?", true),
        //InlineData(@"Interested in free inspection and estimate", true),
        //InlineData(@"We are looking to have our our backyard cleared of carpenter bees. They always appear in the same ap. We are looking to have carpenter bees removed from our back deck. We are located in Danvers. ", true),
        //InlineData("I had the initial service done on 5/17 and while some parts of the house have had less activity we have had areas that haven't tapered off in activity or gotten any better at all. I don't know where the ants are coming from but a majority of them were completely unaffected by the treatment.", false),
        //InlineData("Do you know about how much is it for a 2 bedroom 1 bathroom house with a basement?", true),
        //InlineData("Looking for a quote to help with outside any problem", true),
        //InlineData("Hi.I live in Braintree look for a quote! ", true),
        //InlineData("Where can I get a list of all your ingredients used to kill all bugs", true),
        //InlineData("Do you have a number to a beekeeper near kingsville", false),
        //InlineData("Could you provide quote for formosquito and bugs service? The property address is 20 County Rd Burlington 01803. The lot size is about 0.6 acres", true),
        //InlineData("Hi there! How much is it for service for fleas? They are in my dogs fur and seem to be in the house now.Looking for prices! Thanks! ", true),
        //InlineData("You are coming tomorrow, we are having issues with some rats, flies and yellow jackets.  Feel free to cal me or my husband for details.His number is 631 922 4734 ", false),
        //InlineData("I currently have your service & have been notice some ants in my house", false),
        //InlineData("How much is the ant control price", true),
        //InlineData("I have a wasp nest I need removed in Framingham", true),
        //InlineData(@"Hi, Had one of your folks at my house a couple weeks ago. Problem appeared to clear up (mice), but we are noticing droppings in the same area again. Could you please contact me so we can schedule another visit? Thanks!", false),
        //InlineData("I am interested in getting an estimate for pest control? ", true),
        //InlineData("I was needing to spay our home for bugs", true),
        //InlineData("We are having an ant issue in the house- small ones.", true),
        //InlineData("Hello.I need service at my house. We found a cockroach in the house", true),
        //InlineData("Getting ants", true),
        //InlineData("I want to cancel the contract that I signed last night for 194 La Solis Drive.Please cancel immediately.Thank you.", false),
        //InlineData("Hi! I would like someone to come out and look for wasp/stinging insect nests.I’ve had at least two come into the house", true),
        //InlineData("Currently have a little bit of an ant problem in the house. Looking to get an estimate for you guys to help us out.", true),
        //InlineData("That's ok! I appreciate your help!", false),
        //InlineData("HI I was wondering what the cost would be if you wanted to get out the contract early?", true),
        //InlineData("please call us we have a pest problem at 1051 lonsdale ave central falls", true),
        //InlineData("Requesting bed bug services at my house", true),
        //InlineData("Looking for exterior service for wasps, carpenter bees, ants and spiders.", true),
        //InlineData("Looking for carpenter bee service.", true),
        //InlineData("I have some hornets nests and a mouse in the basement if someone could come take a look sometime.", true),
        //InlineData("Do traps for carpenter bees really work?", true),
        //InlineData("Interested in pest control estimate & possibly take advantage of $50 off", true),
        //InlineData("Carpenter  Bees in New fence", true),
        //InlineData("Service requested", true),
        //InlineData("I am interested in getting an estimate.Wasps seem to be getting into the basement", true),
        //InlineData("Can we get you guys out again to spray because we are getting a lot of ants in the house? Call me back if you need to talk to me", false),
        //InlineData("Need a quote", true),
        //InlineData("I need a service call for bees and hornets", true),
        //InlineData("Good pm.Just wanna ask what is the estimated time for my appointment tomorrow?", false),
        //InlineData("I need someone to retreat our house.  Ants are showing up everywhere around the outside", false),
        //InlineData("I need help with bed bugs. We just moved to another home. We threw out all mattress and bedding that had them on it.Bought new stuff and found one today! I need help.", true),
        //InlineData("Hello! I am just reaching out for a quick quote for service.Our neighbors referred us and with two dogs and two young children, we just wanted to collect information and see if is something we could budget for!", false),
        //InlineData("I need a quote for a full bug spray and check on a 920sq foot home.We’re trying to prevent any bugs and check missing spots for mice.", true),
        //InlineData("Technician was supposed to follow up last Friday", false),
        //InlineData("Looking to get lawn sprayed for fleas and mosquitos", true), // interested in service
        //InlineData("I'm a customer of you home pest service.  I'm interested in the price and details of your yard protection plan.", true), // wants a quote
        //InlineData("How do I go about getting a free estimate and inspection.I need service asap", true), // interested in service
        //InlineData("We need our couch and mattress treated for fleas.We have wood flooring but our puppy has fleas and we need to get it treated", true), // interested in service
        //InlineData("Requesting information for rodent removal/clean up under home ", true), // interested in service
        //InlineData("DO BETTER AND VETTING YOUR EMPLOYEES", false), // Doesn't want service
        //InlineData("branford river spa | 249 east main | branford", true), // Address counts as wanting service
        //InlineData("The technicianwent to the wrong address.I need help! No one is answering my calls!", false), // Current Customer
        //InlineData("Hello | do you work in Simsbury | CT ?", true), // Address counts as wanting service
        //InlineData("Hi I need an agent", true), // Wanting to talk to us counts as wanting service
        //InlineData("Ralph Kemper refers for technician position in Oxford CT", false), // Wants work, not service
        //InlineData("Will texting result in a quicker response than trying to contact you guys via telephone ????", false), // Current Customer
        //InlineData("I live in a apartment building would you guys still come out?", true), // Wants service
        //InlineData("could someone please call me i have disconnected three times.i am a current custmer for a number of years", false), // Current customer
        //InlineData("Can you please have Charlie Langlois call me as soon as possible", false), // Already knows us
        //InlineData("We just received a text saying you would be at our home tomorrow.This notice is to late for me I cannot get off work.Plese call me.", false), // Current customer
        //InlineData("Where is the technician", false), // current customer
        //InlineData("Hello! The bait traps in my attic are empty and my daughter says she’s heard something up there.I’d like to make an appointment for someone to come out. ", false), // current customer
        //InlineData("Need help.", true), // wants service
        //InlineData("please send copy of agreement to my email. ", false), // current customer
        //InlineData("hi i am using tampon", false), // who knows what is going on here
        //InlineData("Hello! We are hearing some activity in the walls at night.Thanks!", true), // wants service
        //InlineData("What time is appointment for? ", false), // current customer
        //InlineData("James Swett", true), // wants service
        //InlineData("I want to make a payment | ", false), // current customer
        //InlineData("I would like to change my tomorrows appt to another day.Any day other than tomorrow is good except 6 / 22… thank you | 36 Wyant Rd | Oxford | Ct", false), // current customer
        //InlineData("Hey there |  | Wanted to reach out because my card number recently changed and I thin you guys may have the old card number to charge.  | Also | I want to schedule another check -in for the house to see if there is any progress.", false), // current customer
        //InlineData("If I send over a picture can you tell me if it is a cockroach I found ?", true), // wants service
        //InlineData("I need to update my credit card", false), // current customer
        //InlineData("I would like to be told via a door hanger each time a visit is made.Thank you | ", false), // current customer
        //InlineData("Text only | Looking to connect with Ian in Guilford today | 155 Sam Hill.I have a car appointment at 8:30 but hope to be back by 10 if not sooner", false), // already knows about us
        //InlineData("I need to change July 3rd appointment", false), // current customer
        //InlineData("Your worker is in my subdi | Please have him to stop back by 6144 Stoneview Avenue", false), // current customer
        //InlineData("Looking to schedule appointment", true), // wants service
        //InlineData("I am looking to schedule an appointment | ", true), // wants service
        //InlineData("Gaadhi", false), // names don't count as wanting service
        //InlineData("I was told I could come in for an interview tomorrow Thursday 13 but I never got a location I should come to for the interview not sure which one to go to if you can give me a text back that’s would be really great", false), // wants employment
        //InlineData("Call me and I explain better thanks", false), // 
        //InlineData("Hi do you have availability this week to look at a house in Worcester please", true), // addresses count as wanting service
        //InlineData("Contact me as soon as possible", true), // I guess they're looking for service
        //InlineData("I need to schedule a visit", false), // current customer -- this isn't new customer language
        //InlineData("Info", true), // wants service
        //InlineData("Need servide", true), // wants service
        //InlineData("Mensajes |", false), // can't tell what's going on
        //InlineData("Can you tell me what treatments you used for bedbugs?", true), // wants services
        //InlineData("Hi!I’m reaching out to see if you’re at all interested in advertising on our cities CENTRO buses. It’s a really great way to reach the entire county as our buses are on the roads 12 - 20 hours per day | 7 days a week and they rotate the routes they travel daily as well!If you’re interested| I’d love to meet!", false), // solicitation
        //InlineData("Hello | I'd like to schedule for the technician to come.", false), // current customer
        //InlineData("vey simple", false), // can't tell what's going on here
        //InlineData("I had an appointment today with Chris at 8am and he is not here yet", false), // current customer
        //InlineData("can you contact me and give me a good design", true), // wants service
        //InlineData("I have a problem with voles/ chipmunks around my property.Tunnels everywhere. Can you help?", true), // wants service
        //InlineData("How long is my contract | 64 brooktree road  |", false), // current customer
        //InlineData("La dirección del lugar gracias", false), // clearly spoke to us before
        //InlineData("This is a test from IM to understand how this works.Thanks!", false), // IM test
        //InlineData("Expected a visit between 4 & 5pm today. As it’s now 5.30pn | could you come tomorriw instead?", false), // current customer
        //InlineData("can you come to 722 Hillside Ave. Glen Ellyn Illinois today?", true), // wants service
        //InlineData("Holame interesa travajar |", false), // wants work
        //InlineData("Do you do work on navarre beach ???", true), // wants service
        //InlineData("I would like to know more information so that I can understand| I am interested in participating | and hope to be accepted and adopted", false), // can't tell what's going on
        //InlineData("I dram come true  be police", false), // can't tell what's going on
        //InlineData("Would I be able to get an appointment tomorrow 5 / 28 / 24", true), // wants service
        //InlineData("Brad came out today to evaluate and provide a treatment for my home. He did a great job!", false), // current customer
        //InlineData("Schedule a apt", true), // wants service
        //InlineData("I will need to fumigate please", true), // wants service
        //InlineData("Had appointment today from 1pm to 6pm.No one has come or called and it is 5:54 pm", false), // current customer
        //InlineData("Nostrum laborum volu", false), // weird latin stuff, like lorem ipsum stuff
        //InlineData("I need someone to come out to the house tomorrow.Is there a way I can set that up online ?", false), // can't tell what's going on
        //InlineData("Hello!  I am with the Town of Emmitsburg.You have a solicitor in Town that is going door to door.They do not have the appropriate permits.  They must cease now.", false), // not interested in service
        //InlineData("Buenos días  | Busco trabajo", false), // looking for work
        //InlineData("treatment", true), // wants service
        //InlineData("Stayed on hold for 18 minutes and don't have that kind of time | ", false), // current customer
        //InlineData("Why was I charge $210.59 yesterday ?", false), // current customer
        //InlineData("Why are your salesmen ringing the door bell after 8 pm?", false), // not interested in service
        //InlineData("What is your pricing ?", true), // wants service
        //InlineData("I was charged in error.", false), // current customer
        //InlineData("Help | ", true), // Wants service
        //InlineData("Do you send door to door salesmen?", false), // Doesn't want service
        //InlineData("Call me!!!", false), // Anger implies that they know us
        //InlineData("There is nobody answering phone.Will have to switch companies if no response", false), // current customer
        //InlineData("test", false), // IM test
        //InlineData("Are you open today (Saturday)?", true), // 
        //InlineData("I’m trying to check and see if I am under a contract.", false), // They either already know us or are currently a customer
        //InlineData("what time they coming today", false), // current customer
        //InlineData("Waiting for a callback for a few days now. Can someone contact me to set up a visit?", false), // current customer
        //InlineData("Can a supervisor please call me? Thank you.", false), // current customer
        //InlineData("I would like to change the card that is used as the auto payment method.", false), // current customer
        //InlineData("Please have Middletown CT office call me", false), // Already know about us
        //InlineData("I need to update my credit card information", false), // current customer
        //InlineData("Please do not come to my house 7/19 as I’m having people outside for a get together.Please confirm. ", false), // current customer
        //InlineData("Cheshire Crossing HOA", true), // addresses count as wanting service
        //InlineData("Please contact me ASAP.", false), // they likely know us already
        //InlineData("I’m still waiting for a phone call and confirmation regarding an appointment that I have scheduled on Monday . ", false), // current customer
        //InlineData("If you send another person to my house at 7:00 T night and I don’t see a car I will call the cops.DONOT even knock on my door at that time of night.I do have an alarm and I will set it off.When I told the person I wasn’t interested he said you haven’t heard what I have to say.Well I don’t let anyone in my house I haven’t asked to come so I would never use you and I will warn others about you", false), // Anger implies knowing us previously
        //InlineData("want to confirm appointment | ", false), // they have an appointment
        //InlineData("I was wondering if you had a technician in Newtown today. ", false), // they know we have technicians
        //InlineData("How much would it be for you coming and see the house", true), // they want a quote
        //InlineData("Hi! I have bites from last night and I have NO idea what I am looking for  . Please help! ", true), // they want help
        //InlineData("Busco trabajo", false), // looking for work
        //InlineData("I need someone to call me.", true), // implies they want service (??)
        //InlineData("2 or more large groundhogs in our lawn at 831 Radio Rd| E-Town |  |  | 2 or more large groundhogs in our lawn at 831 Radio Rd.| E- Town | ", false), // We don't treat groundhogs
        //InlineData("Emergency", true), // implies they want service
        //InlineData("These is my email i think it wasn't taking right  | Bpadillita87@gmail.com", false), // They seem to want to change their email, or we're contacting them through email
        //InlineData("Please contact asap.Thank you", true), // This implies they want service
        //InlineData("I am interested in finding out what ur monthly fee is| please?", true), // wants service
        //InlineData("Good morning| my husband and I were away for a bit.  We returned on Friday.  This morning| while leaving our house we noticed a large nest on our garage.I have an allergy to stings.  As you can imagine| I’m pretty horrified.We also have an older dog that I’m worried about as well.", true), // caller wants service
        //InlineData("My son and I", false), // who knows what's going on here
        //InlineData("I have a raccoon problem", false), // we don't treat raccoons
        //InlineData("What is the fee?", true), // caller wants service
        //InlineData("Hi.Do you serve Locust Grove| VA?", true), // caller wants service
        //InlineData("Hi there| can I make an appt next week for treatment?", true), // Seems the caller is interested in service
        //InlineData("Hello I woke up two days in a row with bite marks small bumps", true), // seems the caller wants service
        //InlineData("Where are you located?", true), // caller seems to want service
        //InlineData("Goodmorning| we were gone the weekend from our apartment and when we returned| droppings were in the bathroom. I have a house dog so I am trying to see pricing", true), // asked about pricing
        //InlineData("Are you located in Lyndonville| New York", true), // interested in service
        //InlineData("Thanks", false), // caller already in contact
        //InlineData("Could I send you a video of what I found on my pillows? Are these bedbug?", true), // wants service
        //InlineData("Hi", false), // can't tell what's going on
        //InlineData("Nesesito una cita para renovar mi pasaporte beliceño estoy embarasada usted porfabor puede ayudarme", false), // sounds like spam -- texter wants help renewing a passport
        //InlineData(".", false), // who knows what's going on here
        //InlineData("We couldn't get anyone out today though| it's a holiday.", false), // this sounds like us, not a potential customer
        //InlineData("Please unsubscribe me", false), // caller already knows about us
        //InlineData("1 room Friday night check in", false), // seems like they might know about us already
        //InlineData("Hi how much do you charge? Grand island ny", true), // caller requesting a quote
        //InlineData("What", false), // who knows what's going on
        //InlineData("I can handle that.  Can you do card for that or I could write a check", false), // caller seems to already be in contact with us
        //InlineData("It's a 3 family property| I am the landlord", true), // caller seems to be interested in service
        //InlineData("Can you see the nest?", true), // caller seems interested in service
        //InlineData("24 hunters church rd liverpool Pa 17045", true), // addresses count as interest
        //InlineData("Hello| can someone please advise when we can expect someone to come today? 244 Tyler Ave Miller Place", false), // current customer
        //InlineData("have something in my house-digging in large plants overnight", true), // interested in services
        //InlineData("Do you guys do anything for chipmunks that are possibly getting behind siding?", false), // we don't treat chipmunks
        //InlineData("I have an appointment today just wondering what time they will be here", false), // current customer
        //InlineData("Derrias Brown", false), // names don't count as interest
        //InlineData("Would like a supervisor to call me concerning some miss communications please have tried to contact talked to 3 different people with no answers and no call backs", false), // current customer
        //InlineData("Do you take care of gnats?", true), // interested in service
        //InlineData("hi", false), // can't tell what's going on
        //InlineData("Hi how much u charge for the process?", true), // wants a quote
        //InlineData("I recently noticed a few dead yellow jacks in my home.  I believe I might have a nest somewhere.  How do you handle this situation if I don’t know where the nest could be?", true), // interested in service
        //InlineData("I'd like to upgrade my payment card | thank you  | Michelly", false), // current customer 
        //InlineData("I havs seen a baby scorpin in my bathroom| what can be done to rid them?", true), // asking for service
        //InlineData("I need a manager to call me. ", false), // current customer
        //InlineData("Hello| when I first moved in with my boyfriend| he said he had a roach problem due to the ex picking up items from the trash| and I’ve tried to buy thing and it’s getting out of hand | I see them everywhere! I’m also interested in the monthly plan| thank you", true), // interested in service
        //InlineData("Do you do back yard control for diggers or mites? Is it pet safe? Thank you!", true), // interested in service
        //InlineData("I dont speak english| but i need exterminated roach", true), // interested in service
        //InlineData("Call me", false), // seems to know about us already
        //InlineData("hello| i was just wondering when i’d have to make the first payment for treatment", true), // current customer
        //InlineData("In the family room and it run into the laundry room", false), // can't tell what's going on
        //InlineData("Looking.Job  in. Rochester NY (Fairport)", false), // looking for work
        //InlineData("New house in Maurice.Just noticed the sound of little feet in the attic or ceiling last night. ", true), // interested in service
        //InlineData("I am seeing greater activity after my first visit.I need to talk to someone about the plan.", false), // already had a service
        //InlineData("where did you notice it?", false), // this sounds like us, not someone else
        //InlineData("Jadon Byers", false), // names don't count as interest
        //InlineData("Do you do apartment?", true), // interested in service
        //InlineData("Hi  | I got a package from Paris at the office please", false), // I don't know what's going on here, but it sounds like an employee, maybe
        //InlineData(" ​Do you do Wood decay fungi treatments? ", false), // we don't treat this
        //InlineData("Hi it’s Thomas| I made an appointment for next week| and someone said they will email me the paperwork| but I never got it yet", false), // caller made an appointment
        //InlineData("How can I reserve igloo for my birthday please?", false), // This is not a service provided
        //InlineData("Can you use a new dependable employee.Please call", false), // caller wants work
        //InlineData("Please take me off your emailing list. Not interested.", false), // apparently we're in contact
        //InlineData("How can I register online to update my payment information", false), // current customer
        //InlineData("Good Afternoon| can you please assist me with terminating my subscription?", false), // current customer
        //InlineData("Trabajo ", false), // caller wants work
        //InlineData("60712", true), // address counts as interest
        //InlineData("We have some critter in our walls in the upstairs bedroom.Need help eradicating this animal.", false), // we don't treat wild animals
        //InlineData("Merci", false), // don't know what's going on here
        //InlineData("Is this stuff save around pets as I have a dog?", true), // interested in service
        //InlineData("Yea I was wondering if you could email not call cuz I’m not home anyways was wondering how much yall charge I have a really bad roach problem", false), // current customer
        //InlineData("Do you do bora care for new construction", false), // not serviced
        //InlineData("I need cockroach Sevice in my house", true), // interested in service
        //InlineData("For a event", false), // not sure what's going on
        //InlineData("If you’re honest| news liberal| and CNN And not fake news news a guy who knows a little bit and 73 in your room", false), // what
        //InlineData("I received a message that I was scheduled for an appt on 10 Nov.I just had an appt on 10 Oct.Why am I being again so soon?", false), // current customer
        //InlineData("My family and are starting to have a roach problem", true), // interested in service
        //InlineData("I didn't get your email  | Confirming address  | pooh4995@hotmail.com", false), // already in contact
        //InlineData("I'm waiting for your reply please", false), // already in contact
        //InlineData("How much is first visit | ", true), // interested in service
        //InlineData("Need a help to control small crakroches  in the kitchen", true), // interested in services for crakroches
        //InlineData("How much would a 550 sq foot one bedroom apt be", true), // interested in service
        //InlineData("Hi| how much do you charge to bomb a basement?", true), // interested in service
    #endregion
        InlineData("Hi hope all is well| I had an appointment scheduled for 11/28/2023 and no has shown up yet", false), // current customer
    #region Below
        //InlineData("Hello.We have a gnawing sound coming from the attic above our garage from some kind of critter.Is this something you can address for us?", true), // interested in service
        //InlineData("I got a message that I have a balance due but can’t find how to log in to get that resolved", false), // current customer
        //Conflict InlineData("Can you please let me know if you come to Calverton thank you", true), // interested in service
        //InlineData("Would like to rent a heater", false), // service not provided
        //InlineData("I would like to send a positive review today I’ve already written it but besides Google or Yelp or Listen 360 can I have your email address and do it that way otherwise I’ll try to do Yelp | Thank You", false), // current customer
        //InlineData("I cannot address this ti after the new year.I have a very horrendous problem.Please let me know what your fee is. Thank you.", true), // interested in service
        //InlineData("We found a dead cockroach in our living room.", true), // interested in service
        //InlineData("Multiple dead bumblebees in basement.Saw them nesting on the outside of house last fall but unable to access...", false), // not treated
        //InlineData("Inside treatment", true), // wants treatment
        //InlineData("Estoy interesada por el trabajo", false), // wants work
        //InlineData("Hola soy estrella", false), // wut
        //InlineData("Hello I’m looking for an exterminator asap.", true), // wants service
        //InlineData("I'm in need of a one-time treatment to satisfy the requirements of a sales contract of our residential home. I am a real estate agent in Mendon and selling our home. I can give much more information during a phone call.", true) // wants service
        //InlineData("How much would this charge", true), // wants service
        //InlineData("How much is a first time treatment for my home", true), // wants service
        //InlineData("Elena Rodríguez", true), // names count
        //InlineData("What is your pricing like?", true), // wants service
        //InlineData("Hi how much for a small room", true), // wants service
        //InlineData("Can you please email (nargue @vetcor.com) my Dec 2023 invoice for Nickel City Animal Hospital(568416)? | ", false), // customer -- invoice
        //Conflict InlineData("Hey | Would you have anyone available today | I live in bartlett", true), // wants service
        //InlineData("Do you have anyone that would come out today | ", false), // wants service -- likely a customer
        //InlineData("I need someone to take care my roach problem", true), // wants service
        //InlineData("Bom dia estou entered ado no em trabalhar", false), // wants work
        //InlineData("Mire que están necesitando personas para Trabajo", false), // wants work
        //InlineData("please call.  I'm trying to make payment and your numbers don't work", false), // current customer
        //InlineData("Can you tell me when a tech will be at my house today?  23 Canal St Winchester Ma", false), // current customer
        //InlineData("Need baiting of vacant house prior to demolition", true), // wants service
        //InlineData("Estoy interesado en el trabajo  | Vivo en East Haven CT", false), // wants work
        //InlineData("I have fixed the problem with my credit card. It should go through now if you re-submit it. Sorry about the inconvenience. | Eric Bello", false), // current customer
        //InlineData("I am looking for an exterminator for a commercial building.", true), // interested in service
        //InlineData("Sorry was so upset with you guys before for not being able to come todsy", false), // current customer
        //InlineData("My son says it will be fine till it’s taken care of", false), // don't know exactly what's going on, but it looks like this person is already in contact with us
        //InlineData("Ocupo el empleo", false), // wants work
        //InlineData("I didn’t see the contract yet.I’m not sure where to look for it.", false), // already in contact
    #endregion
    ]
    #endregion
    public void MessagePatternBillableTest(string contents, bool expected)
    {
        // Assemble
        // Act
        var result = MessagePatterns.Billable(contents);
        var actual = result.Result;
        var matches = result.Matches;
        var noMatch = result.NoMatches;
        var originalInput = result.Input;

        // Assert
        Assert.Equal(expected, actual);
        Assert.Equal(contents, originalInput);
        Assert.NotNull(matches);
        Assert.Equal(string.IsNullOrWhiteSpace(matches), noMatch);
    }
}

#region Unassigned
// Unassigned
//InlineData("Hello| I was just wondering if you had cheaper options than the current plan I'm on.  ", ),
//InlineData("please call this number or 203-918-7440", ),
//InlineData("How save is your product", ),
//InlineData("payment | ", ),
//InlineData("Wanting to look at my subscriptions and possibly change some", ),
//InlineData("Adam Densmore", ),
//InlineData("I need to get a ipm for chicken permits", ),
//InlineData("what is your fee for my house? | 820 Seneca Ln. Carol Stream", ),
//InlineData("One of your employees was freely walking around our house without permission. I don't know if he had the wrong house by mistake| but please stay off our property. We don't subscribe nor are we interested. ", ),
//InlineData("Hola hablo español", ),
//InlineData("I only wanted the free upgrade", ),
//InlineData("I have a servicing appointment today.What time should I be expecting the exterminator to arrive?", ),
//InlineData("Looking for a product to disinfect for scabies.Appreciate any information| thanks", ),
//InlineData("Hi| I need to know if you guys deal with chipmunks.I think I 1 or 2 in my house. I saw 1| but it's possible there are 2. And how much you charge please. We live in Woodbury. My name is Denise", ),
//InlineData("Para comenzar de inmediato", ),
//InlineData("Call me", ),
//InlineData("The rep who came out on Friday February 9 did not put black boxes or any devices in the living room and dining room and kitchen and we are finding droppings daily on tables couch cushion and stove etc.Help", ),
//InlineData("I have a problem with a Prairie dog coming up in my yard| making several holes", ),
//InlineData("Hi| this is a store called Pit Stop at 1501| E North Street Victoria TX 77901-7051.", ),
//InlineData("can you come over today?", ),
//InlineData("How much for treatment", ),
//InlineData("Hello. I'm at 1971 white oak in Algonquin. I just found a dead vole in my basement", ),
//InlineData("I Had an account with you #1077921 Looks like it went into default when we had a CC# number change due to fraud. Is there anyway to reopen the account or is it closed permanently?", ),
//InlineData("I am looking for info", ),
//InlineData("About my payment", ),
//InlineData("Please contact me about automatic payments. I recently had to replace my debit card| which may mean you will not be able to auto-debit from my accocunt.", ),
//InlineData("I need coat for my house", ),
//InlineData("Andrew Cona", ),
//InlineData("Por llamar a ese número", ),
//InlineData("102 Foltim Way|  Congers", ),
//InlineData("I want to make a payment", ),
//InlineData("Hi do you guys take care of nats ?", ),
//InlineData("Can u send me a link to make 1 time payment.. not interested in auto payments..", ),
//InlineData("Quiero saber cuánto cuesta una fumigada", ),
//InlineData("There is a dead animal smell coming from the basement stairs wall", ),
//InlineData("Looking to schedule a walk through of my new rental", ),
//InlineData("Necesitó información", ),
//InlineData("Nesesito trabajo", ),
//InlineData("Roach in basement", ),
//InlineData("I just moved into this house| and it has a pretty bad bedbug infestation", ),
//InlineData("You are right and while they don't pose significant harm to humans| their presence can be a nuisance.", ),
//InlineData("Okay| how long does it take? Just trying to figure out what time I will be out of the house", ),
//InlineData("Evan Kaplan", ),
//InlineData("9565215889", ),
//InlineData("Resent", ),
//InlineData("Yes which day would you like?", ),
//InlineData("You still there? ", ),
//InlineData("Alicia Rodriques", ),
//InlineData("Hi I wanted to see how much y'all plans going for ", ),
//InlineData("Are there cheaper plans available than what I currently have? ", ),
//InlineData("I need an exterminator as soon as possible please", ),
//InlineData("Hi! I made an appointment earlier today for tomorrow and was told I’d be receiving an email that I needed to sign. I have yet to receive an email so I’m thinking you may have had the incorrect email.It is jaclyneevm @gmail.com", ),
//InlineData("its for a bussiness", ),
//InlineData("Hi was is there a pending $862.74 charge on my credit card?", ),
//InlineData("Please give me a call. Tk u", ),
//InlineData("how are you im getting messages that i have a past due balance but im in rolled in automatic payment", ),
//InlineData("How much is one single treatment? And is there a guarantee?", ),
//InlineData("Yo Estoy Buscando Trabajo | ", ),
//InlineData("Yo Estoy Buscando Trabajo", ),
//InlineData("Hello how much would ir be for bed bud treatment", ),
//InlineData("Looking for Cockroach exterminator for a Rental house i manage", ),
//InlineData("Would someone be able to come to my home on Wednesday? I have an issue of some sort of an uninvited guest in my home as well as garage", ),
//InlineData("Food", ),
//InlineData("Box Elders outside condo", ),
//InlineData("Looking for careers in my town. Lafayette| LA 70506", ),
//InlineData("Hello| how are you| good afternoon? I'm looking for a job. I have some experience in creating pigs| cleaning and more. | ", ),
//InlineData("(. home-address.)1003 bayner road(.banier road.)Essex|Maryland|21221(.ph.no.)410-238-6078(.cell.no.)443-563-0207(.message.cell.no.)443-990-5186", ),
//InlineData("Kyle Gorze", ),
//InlineData("Hi I was wondering if you deal with raccoons??", ),
//InlineData("Yuh", ),
//InlineData("Are you hiring? If yes please send information on how to apply. Thank you. ", ),
//InlineData("I live in Redding and I’m looking for an exterminator", ),
//InlineData("Quiero trabajar | ", ),
//InlineData("Hi my girlfriend Emily txted you my names Andrew her boyfriend can you we talk on the phone tomorrow at 12 during my lunch break at work", ),
//InlineData("I have bedbugs. Please help. ", ),
//InlineData("I would like to know if there are like monthly payment plans or something?", ),
//InlineData("Necesito para eliminar chinches hemos encontrado y ntes de q senpropgue quiero saber como hacer | ", ),
//InlineData("About how much would it be", ),
//InlineData("Hi", ),
//InlineData("ellen i love you", ),
//InlineData("Looking for a job", ),
//InlineData("Sens", ),
//InlineData("Hi! I’d like to schedule an appointment to help get rid of a cockroach problem I’m having in my apartment!", ),
//InlineData("22030", ),
//InlineData("Jesse Johnston", ),
//InlineData("Hello | I need to update my payment method. Can you please help with that? I don't see a way to do it online. ", ),
//InlineData("Please call me", ),
//InlineData("Next time when you come| can you leave a card or note so we know when yall come by", ),
//InlineData("Never heard back from you", ),
//InlineData("Hi!We’re having a severe any problem and I think they may have had babies in our heat vents.SOS", ),
//InlineData("Appointment", ),
//InlineData("I have a german roach problem can you get rid of them", ),
//InlineData("I want to update my auto pay with a different credit card please and thank you  |  | address: 197 shellridge dr| east amherst | ny 14051", ),
//InlineData("I am trying to update my CC on file | but not seeing where I can do it on the website |", ),
//InlineData("I would like a copy of all of my receipts please  |", ),
//InlineData("Looking to understand when my contract is up.", ),
//InlineData("Hola buenas tardes", ),
//InlineData("Hi!im looking to get some pricing to perhaps someone come by Saturday Morning..Me interesa información sobre el empleo", ),
//InlineData("Hello | I just want to know exactly where your main office is located ? Are you in Bristol Connecticut or do you just serve Bristol connecticut?", ),
//InlineData("good", ),
//InlineData("good", ),
//InlineData("I will try your products for 45 days | after that we will see how good……", ),
//InlineData("Estoy buscando trabajo y no se inglés", ),
//InlineData("I have bedbugs and need them out immediately!", ),
//InlineData("do you do North Hills of Pittsburgh", ),
//InlineData("Quiero trabajar desde casa", ),
//InlineData("I would like the technician to return to my home. A new nest has appeared", ),
//InlineData("Need an affordable terminate treatment for home and garage - ASAP.Looking for pricing", ),
//InlineData("I need to give you our new credit card number as the one that you had had to be switched due to fraudulent charges.I’m going to include the number you need to use and that is 4806 - 4100 - 1723 - 7800 | 12 / 25.   096", ),
//InlineData("Busco trabajo", ),
//InlineData("Moving into house with lots of webs in basement", ),
//InlineData("I know how to work in the field.", ),
//InlineData("Do you guys do lawn care and applications as well ?", ),
//InlineData("Buenas tardes quisiera aplicar para el trabajo |", ),
//InlineData("How much do you charge", ),
//InlineData("Bid", ),
//InlineData("We have 2 inside dogs and a cat || constantly scratching", ),
//InlineData("Hello! | Is there any availability today for an appointment? |  | Thank you! | Lauren", ),
//InlineData("We had a larvae come out of a ceiling light fixture and are looking for someone to take a look at the issue.", ),
//InlineData("Please | when are you planning to make the Srping visit ? Thanks", ),
//InlineData("Para solicitar empleo", ),
//InlineData("Gnats infestation", ),
//InlineData("Please call so we can discuss my issues and concerns.Thank you", ),
//InlineData("Hello can you tell me how much one application is for about a half acre and I know I mostly just care about inside the fence and along the foundation.Maybe a little bit into the high grass around the property", ),
//InlineData("How much for wood be treatment we have used you in the past", ),
//InlineData("Krolina2601@gmail.com", ),
//InlineData("What is the plan for controlling cicadas?", ),
//InlineData("Help", ),
//InlineData("I have move or voles in my yard.I have a dog.I need a little help.", ),
//InlineData("Advertising says you are in Schenectady ny.Where are you located |", ),
//InlineData("Por favor llámame", ),
//InlineData("Hi | I'm scheduled for a treatment today is there a time frame? ", ),
//InlineData("Tell your shitty door to door salesman to not walk through peoples gardens and lawns.Next time I’m calling the police", ),
//InlineData("Can they get in your mouth and nose ?", ),
//InlineData("Liked “You have a new Leadferno lead in your inbox.”", ),
//InlineData("Please teach your employees to pay attention to No Soliciting Signs.I just had someone from your business knock on my door & I have two signs up", ),
//InlineData("Una pregunta ustedes necesitan trabajadores", ),
//InlineData("I would like to make an appointment", ),
//InlineData("Buenos días | necesito de su servicio | necesito exterminar chinches en mi apartamento.", ),
//InlineData("I recently set up an appointment for this Thursday | May 2nd.What is the earliest arrival time / Timeframe that is scheduled ? Thank you", ),
//InlineData("Hello | I am just looking to see if you have an App", ),
//InlineData("Would you have an idea when 388 main st tonawanda will be arriving tomorrow?", ),
//InlineData("Sorry gave you the wrong phone number Thank you Mike Taylor", ),
//InlineData("Hi...just checking to see when our contract expires.  36 Forrest Ave Lawrence NJ 08648", ),
//InlineData("Trabajo", ),
//InlineData("Just a heads up that the number you all list in your emails is disconnected and the number listed on your website is also not functioning.I was trying to call to schedule a specific time window for indoor treatment at my house.Any guidance here would be helpful…", ),
//InlineData("My bedbug is confined to my bedroom and when I try to sleep| they attacked me. They don’t bother my wife. I’ve had Paramount Pesst Control tried to treat him.Didn’t help much.Any help you can give I would greatly appreciate.", ),
//InlineData("Soy de Mission tendrá algo cercas por aca", ),
//InlineData("I hired you last year.Not positive when but close to know.If you charge me again | I’ll reject it as I haven’t received contact or a treatment.", ),
//InlineData("Nesecito mas information sobre los trabajo y salarios", ),
//InlineData("Update credit card", ),
//InlineData("Hello we have an appointment scheduled today at 65 McDonald rd in colchester.No one has reached out and I was wondering if someone will still be arriving today.", ),
//InlineData("Me puedes. Llamar", ),
//InlineData("How long after treatment before going outside", ),
//InlineData("Hello - i just want to confirm that I should still be expecting treatment today", ),
//InlineData("Need your email", ),
//InlineData("You too", ),
//InlineData("I would like to be home when Angela comes today to 117 Tazewell Rd Newport News 23608.Please ask her to call me before she comes. My wife has a Dr's appointment at 9:30| and should be home by 10:30. | Thanks. ", ),
//InlineData("Can anyone answer the phone", ),
//InlineData("K u know anyone that is", ),
//InlineData("Saludo yo estoy buscando un trabajo yo tengo experiencia para manejar eso", ),
//InlineData("Quit knocking on my door | I am part of the do not knock program in hamilton nj.  I will call the cops next time", ),
//InlineData("I’m sending two referrals to you. My son &daughter. | Joseph Glover & Lisa Oliver., ", ),
//InlineData("Hello can someone come out today", ),
//InlineData("Hey - i was just wondering when my next appointment was |", ),
//InlineData("    update automatic payment method", ),
//InlineData("Hello | we need a free estimation is that posible ?, true) // wants service",
//InlineData("Need a credit line asap", ),
//InlineData("Why is one of your sellers knocking on my door at 8:35 at night ?", ),
//InlineData("I need to change the account from which you are paid.How do I do that?", ),
//InlineData("Hello | my name is victor.I just moved here to new Port news from across country| I am looking to see if you have any job opportunities? Please and thank you.  ", ),
//InlineData("I have a woodchuck under my laundry room. Can you make him not live under there anymore?", ),
//InlineData("Good afternoon | we’d like rejoin your team", ),
//InlineData("Hello!I would like to schedule a time for someone to come out to place some vole traps as we’re having an issue with them.Thank you!", ),
//InlineData("Having trouble updating credit card info", ),
//InlineData("Get rid of x on advertisement that brings you to your site", ),
//InlineData("I have woodchucks under my deck can you get rid of them permanently", ),
//InlineData("Do you help with any problem outside ?", ),
//InlineData("Need my property treated", ),
//InlineData("How much do you generally charge for 14oo square foot home ?", ),
//InlineData("I had got an email about job openings and I was trying to schedule a interview I filled out the application that was in my email so if could get back with me and let me know what more I need to do", ),
//InlineData("I have an appointment today. I would like to confirm the arriving time.  |  | Thank you | -Mengtao", ),
//InlineData("I need help decimating a population of voles that are destroying our backyard | please.", ),
//InlineData("Nesesito trabajar", ),
//InlineData("1312 melrose.i saw a roach", ),
//InlineData("Good Morning and Happy Friday!! |  | I wanted to contact you and congratulate you | as you have advanced to the voting round in the category of |  | Best Exterminator In the 2024 MRT Readers' Choice!  |  Is there an email I can send you more information? kristin.hiers@hearstnp.com", ),
//InlineData("We just find out that there’s bedbugs in one room in our apartment", ),
//InlineData("Hi.I want to set up auto payment method.  Thanks", ),
//InlineData("Please let your reps know that are walking around Cedar Street in Coventry | RI NOT to solicit houses that have NO SOLICITING signs.It is sketchy and gross.THANKS", ),
//InlineData("Do you have employees walking the neighborhood today without a vehicle ?", ),
//InlineData("Nate Morales did an outstanding job!!! Despite the rain he completed his task and was a true professional the entire process!!!", ),
//InlineData("my cat has tape worms.it is infesting my house and we need them gone.", ),
//InlineData("Hi I am trying to reach someone in Human Resources", ),
//InlineData("Open", ),
//InlineData("CONTACT US", ),
//InlineData("Hola bunas taldes", ),
//InlineData("I just noticed there is a pile of sawdust a one place near edge of the foundation", ),
//InlineData("Can I set up an appointment to get traps checked and also my house was power washed last week | so I may need a new application.Thanks| -John", ),
//InlineData("We have come across something digging holes in the back and side yards.We need someone to come out and take a look. 822 Harwood Ave | South Elgin | IL", ),
//InlineData("One of your reps was in my neighborhood over the weekend with a half off promotion.I told him I would need to discuss with my wife who was away for the weekend. Interested in speaking with someone about that offer.Available to talk anytime tomorrow after 3:00.Thanks.", ),
//InlineData("Need treatment at one of my rent properties: 3500 Gaston Dr | Midland | TX.Tenant is seeing critters.  ", ),
//InlineData("We got a move issue and it has gotten to a point where we need professional remedy.", ),
//InlineData("Tom | I’m stuck out between lobby of halls & rooms I’m under the clock you bought recently.", ),
//InlineData("Hi I am interested to build my credit.By the way I am deaf so using text is good.Thanks 🙏", ),
//InlineData("Hi can you tell me the name of the sales employee that was working yesterday in the jagow road / lake mead area? Thank you", ),
#endregion