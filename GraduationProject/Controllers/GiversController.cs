﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GraduationProject.Helpers;
using GraduationProject.Models;
using GraduationProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GraduationProject.Controllers
{
    public class GiversController : Controller
    {
        private GiversService giversService;
        private MembersService membersService;
        private BadgeService badgeService;
        private FileService fileService;
        private readonly IHostingEnvironment hostingEnvironment;

        public GiversController(GiversService giversService, MembersService membersService, BadgeService badgeService, FileService fileService, IHostingEnvironment environment)
        {
            this.giversService = giversService;
            this.membersService = membersService;
            this.badgeService = badgeService;
            this.fileService = fileService;
            this.hostingEnvironment = environment;
        }

        [HttpGet]
        [Authorize]
        [HighlightedMenu(Menu.AddProduct)]
        public async Task<IActionResult> AddProduct()
        {

            await SetBadges();

            return View(new GiversAddProductVM());
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(GiversAddProductVM giversAddProductVM)
        {
            if (!ModelState.IsValid)
                return View(giversAddProductVM);

            if (giversAddProductVM.Picture != null)
            {
                var uniqueFileName = Helper.GetUniqueFileName(giversAddProductVM.Picture.FileName);
                var images = Path.Combine(hostingEnvironment.WebRootPath, "products");
                var filePath = Path.Combine(images, uniqueFileName);
                giversAddProductVM.Picture.CopyTo(new FileStream(filePath, FileMode.Create));
                giversAddProductVM.PictureFileName = uniqueFileName;
            }

            if (giversAddProductVM.Link != null)
            {
                var uniqueFileName = Helper.GetUniqueFileName(giversAddProductVM.Link);
                //fileName = fileService.GetFileName(viewModel.ImageUrl);
                var fileName = fileService.GetFileName(giversAddProductVM.Link);

                await fileService.SaveFileFromUrlAsync(giversAddProductVM.Link, fileName);

                giversAddProductVM.PictureFileName = fileName;
            }

            var giver = await membersService.GetUser(HttpContext.User);
            giversAddProductVM.GiverId = giver.Id;
            giversAddProductVM.Street = giver.Street;
            giversAddProductVM.City = giver.City;
            giversAddProductVM.ZipCode = giver.ZipCode;


            var location = await giversService.GetCoordinates(giver);
            giversAddProductVM.Location = location;

            await giversService.CreateProductAsync(giversAddProductVM);

            return RedirectToAction(nameof(AddProduct));
        }



        [HttpGet]
        [Authorize]
        [HighlightedMenu(Menu.Products)]
        public async Task<IActionResult> Products()
        {
            var giverId = membersService.GetUserId(HttpContext.User);
            var viewModel = new GiversProductsVM
            {
                Claimed = await giversService.GetClaimed(giverId),
                Unclaimed = await giversService.GetUnclaimed(giverId)
            };

            await SetBadges();
            return View(viewModel);
        }

        [HttpGet]
        [HighlightedMenu(Menu.Products)]
        public async Task<IActionResult> ChangeProduct(int id)
        {
            var viewModel = await giversService.GetProduct(id);

            await SetBadges();

            ViewBag.BackButton = true;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProduct(GiversChangeProductVM giversChangeProductVM)
        {
            if (!ModelState.IsValid)
                return View(giversChangeProductVM);

            if (giversChangeProductVM.Picture != null)
            {
                var uniqueFileName = Helper.GetUniqueFileName(giversChangeProductVM.Picture.FileName);
                var images = Path.Combine(hostingEnvironment.WebRootPath, "products");
                var filePath = Path.Combine(images, uniqueFileName);
                giversChangeProductVM.Picture.CopyTo(new FileStream(filePath, FileMode.Create));
                giversChangeProductVM.PictureFileName = uniqueFileName;
            }

            var giver = await membersService.GetUser(HttpContext.User);
            giversChangeProductVM.GiverId = giver.Id;
            giversChangeProductVM.Street = giver.Street;
            giversChangeProductVM.City = giver.City;
            giversChangeProductVM.ZipCode = giver.ZipCode;

            var location = await giversService.GetCoordinates(giver);
            giversChangeProductVM.Location = location;

            await giversService.ChangeProductAsync(giversChangeProductVM);

            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var giverId = membersService.GetUserId(HttpContext.User);

            await giversService.DeleteProduct(id, giverId);
            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> Scan()
        {
            await SetBadges();
            return View(new GiversScanVM());
        }

        [HttpPost]
        public async Task<IActionResult> Scan(string name, string picture, string description, bool notFound)
        {
            var viewModel = new GiversAddProductVM
            {
                ProductName = name,
                Link = picture,
                Description = description,
                NotFound = notFound
            };
            await SetBadges();

            return View(nameof(AddProduct), viewModel);
        }

        private async Task SetBadges()
        {
            var userId = membersService.GetUserId(HttpContext.User);
            ViewBag.BadgeProducts = await badgeService.ProductCount(userId);
            ViewBag.BadgeCart = await badgeService.CartCount(userId);
            ViewBag.BadgeInbox = await badgeService.InboxCount(userId);
        }

    }
}