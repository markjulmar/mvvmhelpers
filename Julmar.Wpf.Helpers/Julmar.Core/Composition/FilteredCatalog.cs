using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace JulMar.Core.Composition
{
    /// <summary>
    /// MEF catalog filter
    /// </summary>
    public class FilteredCatalog : ComposablePartCatalog
    {
        private readonly ComposablePartCatalog _catalogToFilter;
        private readonly Func<ComposablePartDefinition, bool> _partFilter;
        private readonly Func<ExportDefinition, bool> _exportFilter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="catalogToFilter"></param>
        /// <param name="filterDelegate"></param>
        /// <param name="exportDelegate"></param>
        public FilteredCatalog(ComposablePartCatalog catalogToFilter, 
                               Func<ComposablePartDefinition, bool> filterDelegate, 
                               Func<ExportDefinition, bool> exportDelegate)
        {
            _catalogToFilter = catalogToFilter;
            _partFilter = filterDelegate;
            _exportFilter = exportDelegate;
        }

        /// <summary>
        /// Gets the part definitions that are contained in the catalog.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> contained in the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/>.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/> object has been disposed of.</exception>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _catalogToFilter.Parts.SelectMany(
                    part => part.ExportDefinitions, (part, exportDefinition) => new {part, exportDefinition})
                    .Where(t => (_partFilter == null || _partFilter(t.part)) && (_exportFilter == null || _exportFilter(t.exportDefinition)))
                    .Select(t => t.part);
            }
        }

        /// <summary>
        /// Gets a list of export definitions that match the constraint defined by the specified <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition"/> object.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:System.Tuple`2"/> containing the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/> objects and their associated <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> objects for objects that match the constraint specified by <paramref name="definition"/>.
        /// </returns>
        /// <param name="definition">The conditions of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/> objects to be returned.</param><exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/> object has been disposed of.</exception><exception cref="T:System.ArgumentNullException"><paramref name="definition"/> is null.</exception>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return base.GetExports(definition)
                .Where(export => (_partFilter == null || _partFilter(export.Item1)) && (_exportFilter == null || _exportFilter(export.Item2)));
        }
    }
}