﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraduationProject.Hubs;
using GraduationProject.Models;
using GraduationProject.Models.Services;
using GraduationProject.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GraduationProject.Controllers
{
    public class MessagesController : Controller
    {
        private MessagesService messagesService;
        private MembersService membersService;
        private BadgeService badgeService;
        private readonly IHubContext<ChatHub> hubContext;

        public MessagesController(IHubContext<ChatHub> hubContext, MessagesService messagesService, MembersService membersService, BadgeService badgeService)
        {
            this.hubContext = hubContext;
            this.messagesService = messagesService;
            this.membersService = membersService;
            this.badgeService = badgeService;
        }

        [HttpGet]
        [Authorize]
        [HighlightedMenu(Menu.Inbox)]
        public async Task<IActionResult> Inbox()
        {
            var userId = membersService.GetUserId(HttpContext.User);
            var viewModels = await messagesService.GetInbox(userId);

            await SetBadges();

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> StartChatGiver(string receiverId, int productId)
        {
            var giverId = membersService.GetUserId(HttpContext.User);
            await messagesService.StartChat(productId, receiverId, giverId);
            await SetBadges();

            return RedirectToAction("Chat", new { productId });
        }
        [HttpGet]
        public async Task<IActionResult> StartChatReceiver(string giverId, int productId)
        {
            var receiverId = membersService.GetUserId(HttpContext.User);
            await messagesService.StartChat(productId, receiverId, giverId);
            await SetBadges();

            return RedirectToAction("Chat", new { productId });
        }


        [HttpGet]
        [HighlightedMenu(Menu.AddProduct)]
        public async Task<IActionResult> Chat(int productId)
        {
            var userId = membersService.GetUserId(HttpContext.User);
            var viewModel = await messagesService.GetChat(productId, userId);
            if (viewModel == null)
                return RedirectToAction(nameof(Inbox));

            await SetBadges();

            ViewBag.BackButton = true;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Chat(MessagesChatVM messagesChatVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.BackButton = true;
                return View(messagesChatVM);
            }

            return RedirectToAction(nameof(Chat));
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