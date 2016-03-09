using System;
using System.Collections.Generic;
using Seagull.BarTender.Print;
using Label_Printing_with_Bartender.Repository;
using Label_Printing_with_Bartender.Entities;

namespace Label_Printing_with_Bartender.Services
{
    public interface IServices : IDisposable
    {
        /// <summary>
        /// Checks the Bartender Engine runs correctly without exceptions, if so returns true.
        /// </summary>
        /// <returns>True/False</returns>
        bool checkBartenderEngineStarts();

        /// <summary>
        /// Runs the Bartdner engine and returns it.
        /// </summary>
        /// <returns></returns>
        Engine runBartenderEngine();

        /// <summary>
        /// Checks all files paths in the EF database and makes sure they are still avaliable, if not it deletes them fromt he EF database.
        /// </summary>
        /// <param name="repository"></param>
        void fileExistsCheck(IRepository repository);

        /// <summary>
        /// Adds a Format to the EF Database.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        BartenderFormat addFMT(IRepository repository);

        /// <summary>
        /// Uses Bartender Engine to open a format given a format path from the EF database.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        LabelFormatDocument openBartenderFormat(string path, Engine engine);

        /// <summary>
        /// Deletes a format given a Format ID.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        void deleteFMT(IRepository repository, int id);

        /// <summary>
        /// Deletes a Datasource from the EF database given a Dataase ID.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        void deleteDB(IRepository repository, int id);

        /// <summary>
        /// Adds a Datasource to the EF database given a Format to add it to.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="selectedformat"></param>
        void addBartenderDatabase(IRepository repository, BartenderFormat selectedformat);

        /// <summary>
        /// Returns a List of Tables give a datasource connection string.
        /// </summary>
        /// <param name="databaseconnectionstring"></param>
        /// <returns>List of Tables</returns>
        List<DatabaseTable> addTable(string databaseconnectionstring);

        /// <summary>
        /// Returns a List of Columns give a datasource connection string and a table to add them to.
        /// </summary>
        /// <param name="databaseconnectionstring"></param>
        /// <param name="selectedtable"></param>
        /// <returns>List of Columns</returns>
        List<DatabaseColumn> addColumn(string databaseconnectionstring, DatabaseTable selectedtable);

        /// <summary>
        /// Adds a new link to the EF database.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="selectedformat"></param>
        /// <param name="selectedsubstring"></param>
        /// <param name="selecteddatabase"></param>
        /// <param name="selectedtable"></param>
        /// <param name="selectedcolumn"></param>
        void addLink(IRepository repository, BartenderFormat selectedformat, BartenderSubString selectedsubstring, BartenderDatabase selecteddatabase, DatabaseTable selectedtable, DatabaseColumn selectedcolumn);

        /// <summary>
        /// Deletes a Link from the EF database given a Links ID.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        void deleteLink(IRepository repository, int id);

        /// <summary>
        /// Uses Window32 Class to open a file dialog.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="multiselect"></param>
        /// <returns></returns>
        Dictionary<string, string> openFileBox(string filter, bool multiselect);

        /// <summary>
        /// Displays a message to the user give a message to show.
        /// </summary>
        /// <param name="message"></param>
        void showMessageBox(string message);

        /// <summary>
        /// Handles Exceptions.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="err"></param>
        void exceptionHandling(string message, Exception err);

    }
}
