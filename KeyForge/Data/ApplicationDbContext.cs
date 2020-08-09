using System;
using System.Collections.Generic;
using System.Text;
using KeyForge.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KeyForge.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<RegistrationAuthentication> RegistrationAuthentication { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Tournament> Tournament { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<ELOTable> ELOTable { get; set; }
        public DbSet<History> History { get; set; } 
        public DbSet<Attachments> Attachments { get; set; }
        public DbSet<AttachmentAssignment> AttachmentAssignment { get; set; }
    }
}
