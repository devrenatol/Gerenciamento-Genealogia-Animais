using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetoLO.Domain.Entities;

namespace ProjetoLO.Infra.Data.EntitiesConfiguration;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Sexo).IsRequired();
    }
}
