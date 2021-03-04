using Microsoft.EntityFrameworkCore;
using Sistema.Entidades.Ventas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sistema.Datos.Mapping.Ventas
{
    public class DetalleVentaMap : IEntityTypeConfiguration<DetalleVenta>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<DetalleVenta> builder)
        {
            builder.ToTable("detalle_venta")
                .HasKey(d => d.iddetalle_venta);
        }
    }
}
