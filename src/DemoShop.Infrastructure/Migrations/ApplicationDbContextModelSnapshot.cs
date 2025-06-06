﻿// <auto-generated />
using System;
using DemoShop.Domain.Order.Enums;
using DemoShop.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DemoShop.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "order_status", new[] { "created", "completed" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DemoShop.Domain.Order.Entities.OrderEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<OrderStatus>("Status")
                        .HasColumnType("order_status")
                        .HasColumnName("status");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_order");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_order_user_id");

                    b.ToTable("order", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.Order.Entities.OrderItemEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("integer")
                        .HasColumnName("order_id");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer")
                        .HasColumnName("product_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.HasKey("Id")
                        .HasName("pk_order_item");

                    b.HasIndex("OrderId", "ProductId")
                        .IsUnique()
                        .HasDatabaseName("ix_order_item_order_id_product_id");

                    b.ToTable("order_item", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.Product.Entities.CategoryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_category");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_category_name");

                    b.ToTable("category", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.Product.Entities.ImageEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("ProductId")
                        .HasColumnType("integer")
                        .HasColumnName("product_id");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("uri");

                    b.HasKey("Id")
                        .HasName("pk_image");

                    b.HasIndex("ProductId")
                        .HasDatabaseName("ix_image_product_id");

                    b.HasIndex("Uri")
                        .IsUnique()
                        .HasDatabaseName("ix_image_uri");

                    b.ToTable("image", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.Product.Entities.ProductEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.HasKey("Id")
                        .HasName("pk_product");

                    b.ToTable("product", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.ShoppingSession.Entities.CartItemEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("integer")
                        .HasColumnName("product_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.Property<int>("ShoppingSessionId")
                        .HasColumnType("integer")
                        .HasColumnName("shopping_session_id");

                    b.HasKey("Id")
                        .HasName("pk_cart_item");

                    b.HasIndex("ProductId")
                        .HasDatabaseName("ix_cart_item_product_id");

                    b.HasIndex("ShoppingSessionId", "ProductId")
                        .IsUnique()
                        .HasDatabaseName("ix_cart_item_shopping_session_id_product_id");

                    b.ToTable("cart_item", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.ShoppingSession.Entities.ShoppingSessionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_shopping_session");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_shopping_session_user_id");

                    b.ToTable("shopping_session", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.User.Entities.AddressEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Apartment")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("apartment");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("city");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("country");

                    b.Property<string>("Region")
                        .HasColumnType("text")
                        .HasColumnName("region");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("street");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<string>("Zip")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("zip");

                    b.HasKey("Id")
                        .HasName("pk_address");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_address_user_id");

                    b.ToTable("address", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.User.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("KeycloakUserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("keycloak_user_id");

                    b.Property<string>("Phone")
                        .HasColumnType("text")
                        .HasColumnName("phone");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_user_email");

                    b.HasIndex("KeycloakUserId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_keycloak_user_id");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("category_product", b =>
                {
                    b.Property<int>("category_id")
                        .HasColumnType("integer")
                        .HasColumnName("category_id");

                    b.Property<int>("product_id")
                        .HasColumnType("integer")
                        .HasColumnName("product_id");

                    b.HasKey("category_id", "product_id")
                        .HasName("pk_category_product");

                    b.HasIndex("product_id")
                        .HasDatabaseName("ix_category_product_product_id");

                    b.ToTable("category_product", (string)null);
                });

            modelBuilder.Entity("DemoShop.Domain.Order.Entities.OrderEntity", b =>
                {
                    b.HasOne("DemoShop.Domain.User.Entities.UserEntity", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_user_entity_user_id");

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("OrderEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("OrderEntityId");

                            b1.ToTable("order");

                            b1.WithOwner()
                                .HasForeignKey("OrderEntityId")
                                .HasConstraintName("fk_order_order_id");
                        });

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.SoftDelete", "SoftDelete", b1 =>
                        {
                            b1.Property<int>("OrderEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<bool>("Deleted")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("boolean")
                                .HasDefaultValue(false)
                                .HasColumnName("deleted");

                            b1.Property<DateTime?>("DeletedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("deleted_at");

                            b1.HasKey("OrderEntityId");

                            b1.ToTable("order");

                            b1.WithOwner()
                                .HasForeignKey("OrderEntityId")
                                .HasConstraintName("fk_order_order_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("SoftDelete")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DemoShop.Domain.Order.Entities.OrderItemEntity", b =>
                {
                    b.HasOne("DemoShop.Domain.Order.Entities.OrderEntity", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_order_item_order_order_id");

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("OrderItemEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("OrderItemEntityId");

                            b1.ToTable("order_item");

                            b1.WithOwner()
                                .HasForeignKey("OrderItemEntityId")
                                .HasConstraintName("fk_order_item_entity_order_item_entity_id");
                        });

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.SoftDelete", "SoftDelete", b1 =>
                        {
                            b1.Property<int>("OrderItemEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<bool>("Deleted")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("boolean")
                                .HasDefaultValue(false)
                                .HasColumnName("deleted");

                            b1.Property<DateTime?>("DeletedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("deleted_at");

                            b1.HasKey("OrderItemEntityId");

                            b1.ToTable("order_item");

                            b1.WithOwner()
                                .HasForeignKey("OrderItemEntityId")
                                .HasConstraintName("fk_order_item_entity_order_item_entity_id");
                        });

                    b.OwnsOne("DemoShop.Domain.Order.ValueObjects.OrderProduct", "Product", b1 =>
                        {
                            b1.Property<int>("id")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<string>("ProductName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("product_name");

                            b1.Property<string>("ProductThumbnail")
                                .HasColumnType("text")
                                .HasColumnName("product_thumbnail");

                            b1.HasKey("id")
                                .HasName("pk_order_item");

                            b1.ToTable("order_item");

                            b1.WithOwner()
                                .HasForeignKey("id")
                                .HasConstraintName("fk_order_item_order_item_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product")
                        .IsRequired();

                    b.Navigation("SoftDelete")
                        .IsRequired();
                });

            modelBuilder.Entity("DemoShop.Domain.Product.Entities.CategoryEntity", b =>
                {
                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("CategoryEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("CategoryEntityId");

                            b1.ToTable("category");

                            b1.WithOwner()
                                .HasForeignKey("CategoryEntityId")
                                .HasConstraintName("fk_category_entity_category_entity_id");
                        });

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.SoftDelete", "SoftDelete", b1 =>
                        {
                            b1.Property<int>("CategoryEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<bool>("Deleted")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("boolean")
                                .HasDefaultValue(false)
                                .HasColumnName("deleted");

                            b1.Property<DateTime?>("DeletedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("deleted_at");

                            b1.HasKey("CategoryEntityId");

                            b1.ToTable("category");

                            b1.WithOwner()
                                .HasForeignKey("CategoryEntityId")
                                .HasConstraintName("fk_category_entity_category_entity_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("SoftDelete")
                        .IsRequired();
                });

            modelBuilder.Entity("DemoShop.Domain.Product.Entities.ImageEntity", b =>
                {
                    b.HasOne("DemoShop.Domain.Product.Entities.ProductEntity", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("fk_image_product_entity_product_id");

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("ImageEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("ImageEntityId");

                            b1.ToTable("image");

                            b1.WithOwner()
                                .HasForeignKey("ImageEntityId")
                                .HasConstraintName("fk_image_entity_image_entity_id");
                        });

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.SoftDelete", "SoftDelete", b1 =>
                        {
                            b1.Property<int>("ImageEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<bool>("Deleted")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("boolean")
                                .HasDefaultValue(false)
                                .HasColumnName("deleted");

                            b1.Property<DateTime?>("DeletedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("deleted_at");

                            b1.HasKey("ImageEntityId");

                            b1.ToTable("image");

                            b1.WithOwner()
                                .HasForeignKey("ImageEntityId")
                                .HasConstraintName("fk_image_entity_image_entity_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("SoftDelete")
                        .IsRequired();
                });

            modelBuilder.Entity("DemoShop.Domain.Product.Entities.ProductEntity", b =>
                {
                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("ProductEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("ProductEntityId");

                            b1.ToTable("product");

                            b1.WithOwner()
                                .HasForeignKey("ProductEntityId")
                                .HasConstraintName("fk_product_entity_product_entity_id");
                        });

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.SoftDelete", "SoftDelete", b1 =>
                        {
                            b1.Property<int>("ProductEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<bool>("Deleted")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("boolean")
                                .HasDefaultValue(false)
                                .HasColumnName("deleted");

                            b1.Property<DateTime?>("DeletedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("deleted_at");

                            b1.HasKey("ProductEntityId");

                            b1.ToTable("product");

                            b1.WithOwner()
                                .HasForeignKey("ProductEntityId")
                                .HasConstraintName("fk_product_entity_product_entity_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("SoftDelete")
                        .IsRequired();
                });

            modelBuilder.Entity("DemoShop.Domain.ShoppingSession.Entities.CartItemEntity", b =>
                {
                    b.HasOne("DemoShop.Domain.Product.Entities.ProductEntity", "Product")
                        .WithMany("CartItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cart_item_product_product_id");

                    b.HasOne("DemoShop.Domain.ShoppingSession.Entities.ShoppingSessionEntity", "ShoppingSession")
                        .WithMany("CartItems")
                        .HasForeignKey("ShoppingSessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cart_item_shopping_session_entity_shopping_session_id");

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("CartItemEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("CartItemEntityId");

                            b1.ToTable("cart_item");

                            b1.WithOwner()
                                .HasForeignKey("CartItemEntityId")
                                .HasConstraintName("fk_cart_item_entity_cart_item_entity_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("ShoppingSession");
                });

            modelBuilder.Entity("DemoShop.Domain.ShoppingSession.Entities.ShoppingSessionEntity", b =>
                {
                    b.HasOne("DemoShop.Domain.User.Entities.UserEntity", "User")
                        .WithMany("ShoppingSessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shopping_session_user_entity_user_id");

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("ShoppingSessionEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("ShoppingSessionEntityId");

                            b1.ToTable("shopping_session");

                            b1.WithOwner()
                                .HasForeignKey("ShoppingSessionEntityId")
                                .HasConstraintName("fk_shopping_session_entity_shopping_session_entity_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DemoShop.Domain.User.Entities.AddressEntity", b =>
                {
                    b.HasOne("DemoShop.Domain.User.Entities.UserEntity", "User")
                        .WithOne("Address")
                        .HasForeignKey("DemoShop.Domain.User.Entities.AddressEntity", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_address_user_entity_user_id");

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("AddressEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("AddressEntityId");

                            b1.ToTable("address");

                            b1.WithOwner()
                                .HasForeignKey("AddressEntityId")
                                .HasConstraintName("fk_address_entity_address_entity_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DemoShop.Domain.User.Entities.UserEntity", b =>
                {
                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.Audit", "Audit", b1 =>
                        {
                            b1.Property<int>("UserEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<DateTime>("CreatedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("created_at");

                            b1.Property<DateTime>("ModifiedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("updated_at");

                            b1.HasKey("UserEntityId");

                            b1.ToTable("user");

                            b1.WithOwner()
                                .HasForeignKey("UserEntityId")
                                .HasConstraintName("fk_user_entity_user_entity_id");
                        });

                    b.OwnsOne("DemoShop.Domain.Common.ValueObjects.SoftDelete", "SoftDelete", b1 =>
                        {
                            b1.Property<int>("UserEntityId")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<bool>("Deleted")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("boolean")
                                .HasDefaultValue(false)
                                .HasColumnName("deleted");

                            b1.Property<DateTime?>("DeletedAt")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("deleted_at");

                            b1.HasKey("UserEntityId");

                            b1.ToTable("user");

                            b1.WithOwner()
                                .HasForeignKey("UserEntityId")
                                .HasConstraintName("fk_user_entity_user_entity_id");
                        });

                    b.OwnsOne("DemoShop.Domain.User.ValueObjects.PersonName", "PersonName", b1 =>
                        {
                            b1.Property<int>("id")
                                .HasColumnType("integer")
                                .HasColumnName("id");

                            b1.Property<string>("Firstname")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("firstname");

                            b1.Property<string>("Lastname")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("lastname");

                            b1.HasKey("id")
                                .HasName("pk_user");

                            b1.ToTable("user");

                            b1.WithOwner()
                                .HasForeignKey("id")
                                .HasConstraintName("fk_user_user_id");
                        });

                    b.Navigation("Audit")
                        .IsRequired();

                    b.Navigation("PersonName")
                        .IsRequired();

                    b.Navigation("SoftDelete")
                        .IsRequired();
                });

            modelBuilder.Entity("category_product", b =>
                {
                    b.HasOne("DemoShop.Domain.Product.Entities.CategoryEntity", null)
                        .WithMany()
                        .HasForeignKey("category_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_category_product_category_category_id");

                    b.HasOne("DemoShop.Domain.Product.Entities.ProductEntity", null)
                        .WithMany()
                        .HasForeignKey("product_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_category_product_product_product_id");
                });

            modelBuilder.Entity("DemoShop.Domain.Order.Entities.OrderEntity", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("DemoShop.Domain.Product.Entities.ProductEntity", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Images");
                });

            modelBuilder.Entity("DemoShop.Domain.ShoppingSession.Entities.ShoppingSessionEntity", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("DemoShop.Domain.User.Entities.UserEntity", b =>
                {
                    b.Navigation("Address");

                    b.Navigation("Orders");

                    b.Navigation("ShoppingSessions");
                });
#pragma warning restore 612, 618
        }
    }
}
