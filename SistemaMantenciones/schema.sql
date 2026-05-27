-- Script de creación de la base de datos compartida para Mantenciones y Arriendos
CREATE DATABASE IF NOT EXISTS arriendos_mantenciones_db;
USE arriendos_mantenciones_db;

-- Tabla de Vehículos (Compartida)
CREATE TABLE IF NOT EXISTS Vehiculo (
    Codigo VARCHAR(20) PRIMARY KEY,
    Patente VARCHAR(10) NOT NULL UNIQUE,
    Marca VARCHAR(50) NOT NULL,
    Modelo VARCHAR(50) NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    Kilometraje INT NOT NULL,
    Estado VARCHAR(30) NOT NULL, -- Valores: 'Activo', 'Arrendado', 'En Mantencion', 'De Baja'
    PrecioArriendoDiario INT NOT NULL
);

-- Tabla de Mantenciones (Sistema Mantenciones)
CREATE TABLE IF NOT EXISTS Mantencion (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CodigoVehiculo VARCHAR(20) NOT NULL,
    Fecha DATETIME NOT NULL,
    RutMecanico VARCHAR(12) NOT NULL,
    Horas INT NOT NULL,
    FOREIGN KEY (CodigoVehiculo) REFERENCES Vehiculo(Codigo) ON DELETE CASCADE
);

-- Tabla de Clientes (Sistema Arriendos)
CREATE TABLE IF NOT EXISTS Cliente (
    Rut VARCHAR(12) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Direccion VARCHAR(200) NOT NULL
);

-- Tabla de Arriendos (Sistema Arriendos)
CREATE TABLE IF NOT EXISTS Arriendo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CodigoVehiculo VARCHAR(20) NOT NULL,
    RutCliente VARCHAR(12) NOT NULL,
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    PrecioDiario INT NOT NULL,
    PrecioTotal INT NOT NULL,
    FOREIGN KEY (CodigoVehiculo) REFERENCES Vehiculo(Codigo) ON DELETE CASCADE,
    FOREIGN KEY (RutCliente) REFERENCES Cliente(Rut) ON DELETE CASCADE
);
