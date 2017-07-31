using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Citizen.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.MarketplaceOffers = new HashSet<MarketplaceOffer>();
            this.Items = new HashSet<Item>();
            this.Companies = new HashSet<Company>();
        }

        public string Name { get; set; }

        public int Experience { get; set; }

        public int Energy { get; set; }

        public int EnergyRestore { get; set; }

        public decimal Money { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }

        public Country Country { get; set; }

        public UserStorage UserStorage { get; set; }

        public Employment Employment { get; set; }

        public DateTime LastWorked { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<MarketplaceOffer> MarketplaceOffers { get; set; }

        public virtual ICollection<Company> Companies { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public void EnergyRestoreEvent(int ticks)
        {
            var amount = GameSettings.EnergyRestoreEventTickAmount * ticks;
            EnergyRestore = Math.Min(EnergyRestore + amount, GameSettings.EnergyMax);
        }

        public ActionStatus Eat()
        {
            var item = Items.FirstOrDefault(i => i.ItemType == ItemType.Food);

            if (item.Amount <= 0)
                return new ActionStatus(false, "No food available.");

            if (Energy == GameSettings.EnergyMax)
                return new ActionStatus(false, "Energy max.");

            if (EnergyRestore <= 0)
                return new ActionStatus(false, "No energy restore available.");
            
            var energyRestored = Math.Min(GameSettings.FoodEnergyRestore, EnergyRestore);
            var energyRestoreConsumed = Math.Min(GameSettings.FoodEnergyRestore, GameSettings.EnergyMax - Energy);

            Energy = Math.Min(Energy + energyRestored, GameSettings.EnergyMax);
            EnergyRestore = Math.Max(EnergyRestore - energyRestoreConsumed, 0);
            item.Amount -= 1;

            return new ActionStatus(true, "Food has been eaten.");
        }

        public ActionStatus SearchJunkyard()
        {
            if (Energy < GameSettings.SearchJunkyardEnergyCost)
                return new ActionStatus(false, "You do not have enough energy to search junkyard.");

            string junkardSearchSummary;

            Random random = new Random();
            int roll = random.Next(0, 101);

            Energy -= GameSettings.SearchJunkyardEnergyCost;

            // nothing found
            if (roll < 50)
            {
                junkardSearchSummary = string.Format("Found nothing. (-{0} energy)", GameSettings.SearchJunkyardEnergyCost);
            }
            // money found
            else if (roll < 90)
            {
                int moneyRoll = random.Next(0, 101);
                decimal moneyFound;

                // small amount
                if (moneyRoll < 85)
                {
                    moneyFound = random.Next(1, 100) / 100M;
                }
                // medium amount
                else if (moneyRoll < 96)
                {
                    moneyFound = random.Next(10, 100) / 10M;
                }
                // high amount
                else if (moneyRoll < 99)
                {
                    moneyFound = random.Next(1, 50);
                }
                // very high amount
                else
                {
                    moneyFound = random.Next(50, 500);
                }

                Money += moneyFound;

                junkardSearchSummary = string.Format(
                    "You have found some money! (-{0} energy, +{1} money)", 
                    GameSettings.SearchJunkyardEnergyCost,
                    moneyFound);
            }
            // food found
            else if (roll < 93)
            {
                int foodFound = random.Next(1, 3);
                
                var food = Items.FirstOrDefault(i => i.ItemType == ItemType.Food);
                food.Amount += foodFound;

                junkardSearchSummary = string.Format(
                    "You have found some food! (-{0} energy, +{1} food)",
                    GameSettings.SearchJunkyardEnergyCost,
                    foodFound);
            }
            // experience found
            else if (roll < 99)
            {
                int experienceFound = random.Next(1, 5);

                Experience += experienceFound;

                junkardSearchSummary = string.Format(
                    "You have gained experience! (-{0} energy, +{1} experience)",
                    GameSettings.SearchJunkyardEnergyCost,
                    experienceFound);
            }
            // energy restore bonus found
            else
            {
                Energy = GameSettings.EnergyMax;

                junkardSearchSummary = "Your energy has been restored.";
            }

            return new ActionStatus(true, junkardSearchSummary);
            
        }

        public ActionStatus ChangeCountry(Country newCountry)
        {
            if (Money < GameSettings.CountryChangeCost)
                return new ActionStatus(false, "No enough money.");

            Country = newCountry;
            CountryId = newCountry.Id;
            Money -= GameSettings.CountryChangeCost;

            return new ActionStatus(true, "Country has been changed.");
        }

        public int GetItemsAmount()
        {
            return Items.Select(c => c.Amount).Sum();
        }

        public decimal GetStorageExtensionCost()
        {
            // the algorithm resembles compound interests
            var multipier = 1 + (UserStorage.Capacity / 10000M);
            var cost = (multipier * multipier) * UserStorage.Capacity * GameSettings.StorageExtensionCostMultiplier;

            // round cost to the hundreds
            return cost - (cost % 100);
        }

        public ActionStatus ExtendStorage()
        {
            var extensionCost = GetStorageExtensionCost();
            if (Money < extensionCost)
                return new ActionStatus(false, "No enough money to extend storage.");

            Money -= extensionCost;
            UserStorage.Capacity += GameSettings.StorageExtensionCapacity;
            Experience += GameSettings.StorageExtensionExperienceGain;

            var storageExtensionSummary = string.Format(
                "Storage extended successfully. (+{0} storage, -{1} money, +{2} experience)",
                GameSettings.StorageExtensionCapacity,
                extensionCost,
                GameSettings.StorageExtensionExperienceGain);

            return new ActionStatus(true, storageExtensionSummary);

        }

        public ActionStatus AddMarketplaceOffer(ItemType itemType, int amount, decimal price)
        {
            var soldItem = Items.First(i => i.ItemType == itemType);
            var marketPlaceholder = Items.First(i => i.ItemType == ItemType.MarketPlaceholder);
            
            if (price <= 0.00M)
                return new ActionStatus(false, "Invalid price.");

            if (amount <= 0)
                return new ActionStatus(false, "Invalid amount.");

            if (amount > soldItem.Amount)
                return new ActionStatus(false, "Amount is not available.");

            soldItem.Amount -= amount;
            marketPlaceholder.Amount += amount;
            
            var offer = new MarketplaceOffer
            {
                ApplicationUserId = Id,
                ItemType = itemType,
                Amount = amount,
                Price = price
            };

            MarketplaceOffers.Add(offer);

            return new ActionStatus(true, "Offer added succesfully.");
        }

        public ActionStatus EditMarketplaceOffer(int offerId, decimal newPrice)
        {
            if (newPrice <= 0.00M)
                return new ActionStatus(false, "Invalid price.");

            var offer = MarketplaceOffers.FirstOrDefault(m => m.Id == offerId);
            if (offer == null)
                return new ActionStatus(false, "Offer cannot be found.");

            offer.Price = newPrice;

            return new ActionStatus(true, "Offer updated succesfully.");
        }

        public ActionStatus DeleteMarketplaceOffer(int offerId)
        {
            var offer = MarketplaceOffers.FirstOrDefault(m => m.Id == offerId);
            if (offer == null)
                return new ActionStatus(false, "Offer cannot be found.");
            
            var soldItem = Items.First(i => i.ItemType == offer.ItemType);
            var marketPlaceholder = Items.First(i => i.ItemType == ItemType.MarketPlaceholder);

            soldItem.Amount += offer.Amount;
            marketPlaceholder.Amount -= offer.Amount;

            MarketplaceOffers.Remove(offer);

            return new ActionStatus(true, "Offer removed succesfully.");
        }

        public ActionStatus BuyMarketplaceOffer(MarketplaceOffer offer, int amount)
        {
            if (offer == null)
                return new ActionStatus(false, "Offer cannot be found.");

            if (amount <= 0)
                return new ActionStatus(false, "Invalid amount.");
            
            if (amount > offer.Amount)
                return new ActionStatus(false, "Amount not available.");

            var transactionCost = amount * offer.Price;

            if(Money < transactionCost)
                return new ActionStatus(false, "No enough money.");
            
            if (GetItemsAmount() + amount > UserStorage.Capacity)
                return new ActionStatus(false, "No enough space in storage.");

            var seller = offer.ApplicationUser;

            var sellerOfferItem = seller.Items.FirstOrDefault(i => i.ItemType == offer.ItemType);
            var sellerPlaceholerItem = seller.Items.FirstOrDefault(i => i.ItemType == ItemType.MarketPlaceholder);

            var buyerOfferItem = Items.FirstOrDefault(i => i.ItemType == offer.ItemType);

            sellerPlaceholerItem.Amount -= amount;
            offer.Amount -= amount;
            buyerOfferItem.Amount += amount;

            seller.Money += transactionCost;
            Money -= transactionCost;

            return new ActionStatus(true, "Items bought successfully.");
        }

        public ActionStatus JobApply(JobOffer jobOffer)
        {
            if (jobOffer == null)
                return new ActionStatus(false, "Job offer not available.");

            if (Employment != null)
                return new ActionStatus(false, "You do have a job already.");

            var employment = new Employment()
            {
                ApplicationUser = this,
                ApplicationUserId = Id,
                Company = jobOffer.Company,
                CompanyId = jobOffer.CompanyId,
                DaysWorked = 0,
                Salary = jobOffer.Salary
            };

            jobOffer.Company.Employments.Add(employment);
            jobOffer.Company.JobOffers.Remove(jobOffer);

            return new ActionStatus(true, "You have been hired successfully.");
        }

        public ActionStatus JobResign(Employment employment)
        {
            if (employment == null)
                return new ActionStatus(false, "Job does not exist.");

            if (Employment == null)
                return new ActionStatus(false, "You do not have a job.");

            if (Employment.Id != employment.Id)
                return new ActionStatus(false, "You do not work here.");

            var company = employment.Company;
            company.Employments.Remove(employment);
            Employment = null;

            return new ActionStatus(true, "You have successfully resigned from a job.");
        }

        public ActionStatus Work()
        {
            if (Employment == null)
                return new ActionStatus(false, "You do not have a job.");

            if (Energy < GameSettings.WorkEnergyCost)
                return new ActionStatus(false, "You do not have enough energy to work.");

            var workTimeDiff = DateTime.Now - LastWorked;
            var timeLeftToNextWork = GameSettings.WorkInterval - (DateTime.Now - LastWorked);
            var message = string.Format(
                "You have already worked today. Please try again in {0} hours, {1} minutes and {2} seconds.", 
                timeLeftToNextWork.Hours,
                timeLeftToNextWork.Minutes,
                timeLeftToNextWork.Seconds);

            if (timeLeftToNextWork > TimeSpan.FromHours(0))
                return new ActionStatus(false, message);

            var companyOwner = Employment.Company.Owner;

            if (companyOwner.Money < Employment.Salary)
                return new ActionStatus(false, "Employer owner does not have enough money for your salary.");

            var product = Employment.Company.Product;
            var source = Employment.Company.Source;
            var companyOwnerProduct = Employment.Company.Owner.Items.FirstOrDefault(item => item.ItemType == product);
            var companyOwnerSource = Employment.Company.Owner.Items.FirstOrDefault(item => item.ItemType == source);

            if (source != ItemType.Nil)
            {
                 if (companyOwnerSource.Amount <= GameSettings.CompanyWorkSource)
                    return new ActionStatus(false, "Employer does not have required sources in his storage.");
            }

            LastWorked = DateTime.Now;

            Energy -= GameSettings.WorkEnergyCost;

            companyOwnerProduct.Amount += GameSettings.CompanyWorkProduct;
            if (source != ItemType.Nil)
            {
                companyOwnerSource.Amount -= GameSettings.CompanyWorkSource;
            }

            Money += Employment.Salary;
            companyOwner.Money -= Employment.Salary;

            Employment.DaysWorked += 1;

            Experience += GameSettings.WorkExperienceGain;

            string workSummary = string.Format(
                "Worked succesfully. (-{0} energy, +{1} money, +{2} experience, {3} days worked so far)", 
                GameSettings.WorkEnergyCost, 
                Employment.Salary, 
                GameSettings.WorkExperienceGain,
                Employment.DaysWorked);

            return new ActionStatus(true, workSummary);
        }

        public ActionStatus CreateCompany(string name, ItemType product)
        {
            if (Money <= GameSettings.CompanyCost)
                return new ActionStatus(false, "No enough money.");

            Money -= GameSettings.CompanyCost;
            Experience += GameSettings.CompanyExperienceGain;

            var company = new Company
            {
                Name = name,
                Product = product,
                Source = GetSourceForProduct(product),
                OwnerId = Id,
                MaxEmployments = GameSettings.CompanyMaxWorkers
            };

            Companies.Add(company);
            
            var companyCreateSummary = string.Format(
                "Company: {0} created. (+{1} experience, -{2} money)",
                name,
                GameSettings.CompanyExperienceGain,
                GameSettings.CompanyCost
                );

            return new ActionStatus(true, companyCreateSummary);
        }

        public ActionStatus DeleteCompany(Company company)
        {
            var userCompany = Companies.FirstOrDefault(c => c.Id == company.Id);
            if(userCompany == null)
                return new ActionStatus(false, "You don't own this company.");

            foreach (var employment in userCompany.Employments.ToList())
            {
                userCompany.Employments.Remove(employment);
            }

            foreach (var jobOffer in userCompany.JobOffers.ToList())
            {
                userCompany.JobOffers.Remove(jobOffer);
            }

            Companies.Remove(userCompany);
            return new ActionStatus(true, "Company deleted succesfully.");

        }

        public ItemType GetSourceForProduct(ItemType product)
        {
            if (product == ItemType.Food)
                return ItemType.Grain;

            else
                return ItemType.Nil;
        }

        public int GetWorkersAndOffersForCompany(Company company)
        {
            var workers = company.Employments.Count();
            var jobOffers = company.JobOffers.Count();
            return workers + jobOffers;
        }

        public ActionStatus FireWorker(Company company, Employment employment)
        {
            var userCompany = Companies.FirstOrDefault(c => c.Id == company.Id);
            if (userCompany == null)
                return new ActionStatus(false, "You don't own this company.");

            var searchEmployment = userCompany.Employments.FirstOrDefault(e => e.Id == employment.Id);
            if(searchEmployment == null)
                return new ActionStatus(false, "No such employment.");

            userCompany.Employments.Remove(searchEmployment);
            return new ActionStatus(true, "Worker fired succesfully.");
        }

        public ActionStatus AddJobOffer(Company company, decimal salary)
        {
            if(salary <= 0.00M)
                return new ActionStatus(false, "Invalid salary.");

            if(GetWorkersAndOffersForCompany(company) >= GameSettings.CompanyMaxWorkers)
                return new ActionStatus(false, "You cannot add more workers or job offers.");

            var userCompany = Companies.FirstOrDefault(c => c.Id == company.Id);
            if (userCompany == null)
                return new ActionStatus(false, "You don't own this company.");

            var jobOffer = new JobOffer
            {
                CompanyId = company.Id,
                Salary = salary
            };

            userCompany.JobOffers.Add(jobOffer);
            return new ActionStatus(true, "Job offer added succesfully.");
        }

        public ActionStatus DeleteJobOffer(Company company, JobOffer jobOffer)
        {
            var userCompany = Companies.FirstOrDefault(c => c.Id == company.Id);
            if (userCompany == null)
                return new ActionStatus(false, "You don't own this company.");

            var searchJobOffer = userCompany.JobOffers.FirstOrDefault(e => e.Id == jobOffer.Id);
            if (searchJobOffer == null)
                return new ActionStatus(false, "No such job offer.");

            userCompany.JobOffers.Remove(jobOffer);
            return new ActionStatus(true, "Job offer deleted succesfully.");
        }
    }
}
