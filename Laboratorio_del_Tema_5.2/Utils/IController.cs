using System;
using System.Collections.Generic;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Interfaz base para todos los controllers.
    /// </summary>
    public interface IController<T>
    {
        /// <summary>
        /// Crea un nuevo registro.
        /// </summary>
        bool Create(T entity);

        /// <summary>
        /// Obtiene todos los registros.
        /// </summary>
        List<T> Read();

        /// <summary>
        /// Actualiza un registro existente.
        /// </summary>
        bool Update(T entity);

        /// <summary>
        /// Elimina un registro por su ID.
        /// </summary>
        bool Delete(int id);
    }

    /// <summary>
    /// Interfaz para controllers que soportan busqueda por ID.
    /// </summary>
    public interface IControllerWithReadById<T> : IController<T>
    {
        /// <summary>
        /// Obtiene un registro especifico por su ID.
        /// </summary>
        T ReadById(int id);
    }

    /// <summary>
    /// Interfaz para controllers con operaciones JOIN.
    /// </summary>
    public interface IControllerWithJoins
    {
        // Los metodos especificos se definen en cada controller
    }
}