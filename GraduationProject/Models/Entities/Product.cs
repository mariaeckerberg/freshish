﻿using System;
using System.Collections.Generic;
using GeoAPI.Geometries;

namespace GraduationProject.Models.Entities
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Freshness { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Description { get; set; }
        public DateTime PickUpDate1 { get; set; }
        public DateTime PickUpDate2 { get; set; }
        public bool Claimed { get; set; }
        public bool Collected { get; set; }
        public string Picture { get; set; }
        public DateTime PublishDate { get; set; }
        public string GiverId { get; set; }
        public string ReceiverId { get; set; }
        public IGeometry Location { get; set; }

        public virtual AspNetUsers Giver { get; set; }
        public virtual AspNetUsers Receiver { get; set; }
    }
}
