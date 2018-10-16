﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplicationRabbitMQ.Models;
using WebApplicationRabbitMQ.Services;

namespace WebApplicationRabbitMQ.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMessageSender _messageSender;
        public  HomeController(IMessageSender messageSender)
        {
          this._messageSender = messageSender;
        }
        public IActionResult Index()
        {
          ViewBag.Message=   this._messageSender.SendMessage("You have accessed");

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult SendReceive()
        {
            ViewBag.Title = "Send and Receive Messages with RabbitMQ";
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}