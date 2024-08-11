using BSP_Using_AI.DetailsModify.Filters.IIRFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BSP_Using_AI.DetailsModify.FormDetailsModify;

namespace Biological_Signal_Processing_Using_AI.DetailsModify.Filters
{
    public abstract class FilterBase
    {
        public string Name { get; set; }
        public bool _activated { get; set; } = true;
        public int _sortOrder { get; set; }

        public FilteringTools _ParentFilteringTools { get; set; }

        public Control _FilterControl;
        public bool _ignoreEvent { get; set; } = false;

        public abstract Control InitializeFilterControl();
        public abstract (double[] filteredSignal, bool reloadSignal) ApplyFilter(double[] filteredSamples, bool forceApply, bool showResultsInChart);
        public abstract void Activate(bool activate);

        public abstract FilterBase Clone(FilteringTools filteringTools);

        public void CloneBase(FilterBase sourceFilter)
        {
            // Clone filter basic properties
            Name = sourceFilter.Name;
            _activated = sourceFilter._activated;
            _sortOrder = sourceFilter._sortOrder;
            _ignoreEvent = sourceFilter._ignoreEvent;
        }

        /// <summary>
        /// Insert filter in filters dictionary and filters flow layout panel
        /// </summary>
        /// <param name="filtersFlowLayout">if null, then insert filter in dictionary only.</param>
        public void InsertFilter(FlowLayoutPanel filtersFlowLayout)
        {
            // Set the order of the filter
            if (_ParentFilteringTools._FiltersDic.Count > 0)
                _sortOrder = _ParentFilteringTools._FiltersDic.Max(filter => filter.Value._sortOrder);
            _sortOrder++;
            // Append order of the filter to its name if the name already exists
            if (_ParentFilteringTools._FiltersDic.ContainsKey(Name))
                Name += _sortOrder;

            // Initialize the filter user control
            _FilterControl = InitializeFilterControl();

            // Insert the filter in FiltersDic
            _ParentFilteringTools._FiltersDic.Add(Name, this);

            // Insert the filter control in filtersFlowLayout
            filtersFlowLayout?.Controls.Add(_FilterControl);
            // Apply filtering
            _ParentFilteringTools?.ApplyFilters(false);
        }

        /// <summary>
        /// Remove filter from filters dictionary and filters flow layout panel
        /// </summary>
        /// <param name="filtersFlowLayout">if null, then remove filter from dictionary only.</param>
        public void RemoveFilter()
        {
            // Remove the filter from FiltersDic
            if (_ParentFilteringTools._FiltersDic.ContainsKey(Name))
                _ParentFilteringTools._FiltersDic.Remove(Name);
            // Remove filter from flow layout panel
            if (_FilterControl != null)
            {
                FlowLayoutPanel filtersFlowLayout = (FlowLayoutPanel)_FilterControl.Parent;
                if (filtersFlowLayout != null)
                    filtersFlowLayout.Controls.Remove(_FilterControl);
            }
            // Apply filtering
            _ParentFilteringTools?.ApplyFilters(false);
        }

        public void SetSortOrder(int order)
        {
            // Sort filters according to _sortOrder
            List<FilterBase> sortedFilters = _ParentFilteringTools._FiltersDic.OrderBy(filter => filter.Value._sortOrder).Select(filter => filter.Value).ToList();
            // Set this filter's new sort order
            _sortOrder = sortedFilters[order]._sortOrder;
            if (!Name.Equals(GetType().Name))
                SetName(GetType().Name + _sortOrder);
            // Update sort order of the next filters
            for (int i = order; i < sortedFilters.Count; i++)
                if (sortedFilters[i] != this)
                {
                    sortedFilters[i]._sortOrder++;
                    if (!sortedFilters[i].Name.Equals(sortedFilters[i].GetType().Name))
                        sortedFilters[i].SetName(sortedFilters[i].GetType().Name + sortedFilters[i]._sortOrder);
                }
            // Set the order of the filter control in filtersFlowLayout
            if (_FilterControl != null)
                if (_FilterControl.Parent != null)
                    ((FlowLayoutPanel)_FilterControl.Parent).Controls.SetChildIndex(_FilterControl, order);
        }
        public void SetName(string name)
        {
            // Update filters dictionary
            if (_ParentFilteringTools._FiltersDic.ContainsKey(Name))
            {
                _ParentFilteringTools._FiltersDic.Remove(Name);
                _ParentFilteringTools._FiltersDic.Add(name, this);
            }
            // Update control's name
            if (_FilterControl != null)
            {
                _FilterControl.Name = name;
                // Check if the control is IIRFilterUserControl
                if (_FilterControl is IIRFilterUserControl)
                    // Update the label of the filter
                    (_FilterControl as IIRFilterUserControl).nameFilterLabel.Text = name;
            }

            Name = name;
        }

        public void ActivateGenerally(bool activate)
        {
            _activated = activate;
            // Update the control
            if (_FilterControl != null && !_ignoreEvent)
            {
                _ignoreEvent = true;
                Activate(activate);
                _ignoreEvent = false;
            }
            // Apply filtering
            _ParentFilteringTools?.ApplyFilters(false);
        }
    }
}
