-- Esquema para la base de datos de mantenciones
CREATE DATABASE IF NOT EXISTS mantenciones;
USE mantenciones;

CREATE TABLE IF NOT EXISTS vehiculo (
    codigo VARCHAR(20) PRIMARY KEY,
    patente VARCHAR(10) NOT NULL UNIQUE,
    marca VARCHAR(50) NOT NULL,
    modelo VARCHAR(50) NOT NULL,
    tipo VARCHAR(50) NOT NULL,
    kilometraje INT NOT NULL,
    estado VARCHAR(30) NOT NULL,
    precioArriendoDiario INT NOT NULL
);

CREATE TABLE IF NOT EXISTS mantenicion (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigoVehiculo VARCHAR(20) NOT NULL,
    fecha DATETIME NOT NULL,
    horas INT NOT NULL,
    descripcion VARCHAR(200) NOT NULL,
    FOREIGN KEY (codigoVehiculo) REFERENCES vehiculo(codigo) ON DELETE CASCADE
);

-- Esquema para la base de datos de arriendos
CREATE DATABASE IF NOT EXISTS arriendos;
USE arriendos;

CREATE TABLE IF NOT EXISTS cliente (
    rut VARCHAR(12) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(200) NOT NULL
);

CREATE TABLE IF NOT EXISTS arriendo (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigoVehiculo VARCHAR(20) NOT NULL,
    rutCliente VARCHAR(12) NOT NULL,
    fechaInicio DATETIME NOT NULL,
    fechaFin DATETIME NOT NULL,
    precioDiario INT NOT NULL,
    precioTotal INT NOT NULL,
    estado VARCHAR(20) NOT NULL,
    FOREIGN KEY (rutCliente) REFERENCES cliente(rut) ON DELETE CASCADE
);
