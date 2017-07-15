using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

        public virtual ICollection<Item> Items { get; set; }

        public virtual ICollection<MarketplaceOffer> MarketplaceOffers { get; set; }

        public virtual ICollection<Company> Companies { get; set; }

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

        public ActionStatus CreateCompany(string name, ItemType product)
        {
            if (Money <= GameSettings.CompanyCost)
                return new ActionStatus(false, "No enough money.");

            Money -= GameSettings.CompanyCost;

            var company = new Company
            {
                Name = name,
                Product = product,
                Source = GetSourceForProduct(product),
                OwnerId = Id,
                MaxEmployments = GameSettings.CompanyMaxWorkers
            };

            Companies.Add(company);

            return new ActionStatus(true, "Company created succesfully.");
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
