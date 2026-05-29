-- Script para crear la base de datos del Sistema de Recursos Humanos
CREATE DATABASE IF NOT EXISTS rrhh_db;
USE rrhh_db;

CREATE TABLE IF NOT EXISTS Mecanico (
    Rut VARCHAR(12) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    SueldoBase INT NOT NULL,
    ValorHora INT NOT NULL
);
