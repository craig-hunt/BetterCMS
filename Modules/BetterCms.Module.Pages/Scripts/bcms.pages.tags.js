﻿/*jslint unparam: true, white: true, browser: true, devel: true */
/*global define, console */

bettercms.define('bcms.pages.tags', ['bcms.jquery', 'bcms', 'bcms.dynamicContent', 'bcms.siteSettings', 'bcms.inlineEdit', 'bcms.grid', 'bcms.ko.extenders', 'bcms.jquery.autocomplete'],
    function ($, bcms, dynamicContent, siteSettings, editor, grid, ko, autocomplete) {
    'use strict';

    var tags = {},
        selectors = {
            deleteTagLink: 'a.bcms-icn-delete',
            addTagButton: '#bcms-site-settings-add-tag',
            tagName: 'a.bcms-tag-name',
            tagOldName: 'input.bcms-tag-old-name',
            tagNameEditor: 'input.bcms-tag-name',
            tagsListForm: '#bcms-tags-form',
            tagsSearchButton: '#bcms-tags-search-btn',
            tagsSearchField: '.bcms-search-query',
            
            deleteCategoryLink: 'a.bcms-icn-delete',
            addCategoryButton: '#bcms-site-settings-add-category',
            categoryName: 'a.bcms-category-name',
            categoryOldName: 'input.bcms-category-old-name',
            categoryNameEditor: 'input.bcms-category-name',
            categoriesListForm: '#bcms-categories-form',
            categoriesSearchButton: '#bcms-categories-search-btn',
            categoriesSearchField: '.bcms-search-query',
    },
        links = {
            loadSiteSettingsCategoryListUrl: null,
            loadSiteSettingsTagListUrl: null,
            saveTagUrl: null,
            deleteTagUrl: null,
            saveCategoryUrl: null,
            deleteCategoryUrl: null,
            tagSuggestionSeviceUrl: null
        },
        globalization = {
            confirmDeleteTagMessage: 'Delete tag?',
            confirmDeleteCategoryMessage: 'Delete category?'
        };

    /**
    * Assign objects to module.
    */
    tags.links = links;
    tags.globalization = globalization;

    /**
    * Retrieves tag field values from table row.
    */
    tags.getTagData = function(row) {
        var tagId = row.find(selectors.deleteTagLink).data('id'),
            tagVersion = row.find(selectors.deleteTagLink).data('version'),
            name = row.find(selectors.tagNameEditor).val();

        return {
            Id: tagId,
            Version: tagVersion,
            Name: name
        };
    };

    /**
    * Search site settings tags
    */
    tags.searchSiteSettingsTags = function (form, container) {
        grid.submitGridForm(form, function (data) {
            siteSettings.setContent(data);
            tags.initSiteSettingsTagsEvents(data);   
            var searchInput = container.find(selectors.tagsSearchField);
            grid.focusSearchInput(searchInput);
        });
    };

    /**
    * Initializes site settings tags list and list items events
    */
    tags.initSiteSettingsTagsEvents = function () {
        var dialog = siteSettings.getModalDialog(),
            container = dialog.container;
        
        var form = dialog.container.find(selectors.tagsListForm);
        grid.bindGridForm(form, function (data) {
            siteSettings.setContent(data);
            tags.initSiteSettingsTagsEvents(data);
        });

        form.on('submit', function (event) {
            event.preventDefault();
            tags.searchSiteSettingsTags(form, container);
            return false;
        });

        container.find(selectors.tagsSearchButton).on('click', function () {
            tags.searchSiteSettingsTags(form, container);
        });

        container.find(selectors.addTagButton).on('click', function () {
            editor.addNewRow(container);
        });
        
        editor.initialize(container, {
            saveUrl: links.saveTagUrl,
            deleteUrl: links.deleteTagUrl,
            onSaveSuccess: tags.setTagFields,
            rowDataExtractor: tags.getTagData,
            deleteRowMessageExtractor: function (rowData) {
                return $.format(globalization.confirmDeleteTagMessage, rowData.Name);
            }
        });
    };

    /**
    * Set values, returned from server to row fields
    */
    tags.setTagFields = function (row, json) {
        if (json.Data) {
            row.find(selectors.tagName).html(json.Data.Name);
            row.find(selectors.tagNameEditor).val(json.Data.Name);
            row.find(selectors.tagOldName).val(json.Data.Name);
        }
    };

    /**
    * Retrieves category field values from table row.
    */
    tags.getCategoryData = function (row) {
        var categoryId = row.find(selectors.deleteCategoryLink).data('id'),
            categoryVersion = row.find(selectors.deleteCategoryLink).data('version'),
            name = row.find(selectors.categoryNameEditor).val();

        return {
            Id: categoryId,
            Version: categoryVersion,
            Name: name
        };
    };

    /**
    * Search site settings categories
    */
    tags.searchSiteSettingsCategories = function (form, container) {
        grid.submitGridForm(form, function (data) {
            siteSettings.setContent(data);
            tags.initSiteSettingsCategoriesEvents(data);                  
            var searchInput = container.find(selectors.categoriesSearchField);
            grid.focusSearchInput(searchInput);
        });
    };

    /**
    * Initializes site settings categories list and list items events
    */
    tags.initSiteSettingsCategoriesEvents = function () {
        var dialog = siteSettings.getModalDialog(),
            container = dialog.container;

        var form = dialog.container.find(selectors.categoriesListForm);
        grid.bindGridForm(form, function (data) {
            siteSettings.setContent(data);
            tags.initSiteSettingsCategoriesEvents(data);
        });

        form.on('submit', function (event) {
            event.preventDefault();
            tags.searchSiteSettingsCategories(form, container);
            return false;
        });

        container.find(selectors.categoriesSearchButton).on('click', function () {
            tags.searchSiteSettingsCategories(form, container);
        });

        container.find(selectors.addCategoryButton).on('click', function () {
            editor.addNewRow(container);
        });

        editor.initialize(container, {
            saveUrl: links.saveCategoryUrl,
            deleteUrl: links.deleteCategoryUrl,
            onSaveSuccess: tags.setCategoryFields,
            rowDataExtractor: tags.getCategoryData,
            deleteRowMessageExtractor: function (rowData) {
                return $.format(globalization.confirmDeleteCategoryMessage, rowData.Name);
            }
        });
    };

    /**
    * Set values, returned from server to row fields
    */
    tags.setCategoryFields = function (row, json) {
        if (json.Data) {
            row.find(selectors.categoryName).html(json.Data.Name);
            row.find(selectors.categoryNameEditor).val(json.Data.Name);
            row.find(selectors.categoryOldName).val(json.Data.Name);
        }
    };
    
    /**
      * Loads site settings category list.
      */
    tags.loadSiteSettingsCategoryList = function () {
        dynamicContent.bindSiteSettings(siteSettings, links.loadSiteSettingsCategoryListUrl, {
            contentAvailable: function () {
                tags.initSiteSettingsCategoriesEvents();
            }
        });
    };

    /**
      * Loads site settings tag list.
      */
    tags.loadSiteSettingsTagList = function () {
        dynamicContent.bindSiteSettings(siteSettings, links.loadSiteSettingsTagListUrl, {
            contentAvailable: function () {
                tags.initSiteSettingsTagsEvents();
            }
        });
    };

    /**
    * Tags list view model
    */
    tags.TagsListViewModel = function(tagsList) {
        var self = this;

        self.isExpanded = ko.observable(true);
        self.tags = ko.observableArray();
        self.newTag = ko.observable().extend({ maxLength: { maxLength: ko.maxLength.name } });

        if (tagsList) {
            for (var i = 0; i < tagsList.length; i ++) {
                if (tagsList[i].Value && tagsList[i].Key) {
                    self.tags.push(new tags.TagViewModel(self, tagsList[i].Value, tagsList[i].Key));
                } else {
                    self.tags.push(new tags.TagViewModel(self, tagsList[i]));
                }
            }
        }

        self.expandCollapse = function () {
            self.isExpanded(!self.isExpanded());
            self.clearTag();
        };

        self.addTag = function() {
            var newTag = $.trim(self.newTag());
            if (newTag && !self.alreadyExists(newTag) && !self.newTag.hasError()) {
                var tagViewModel = new tags.TagViewModel(self, newTag);
                self.tags.push(tagViewModel);
            }
            self.clearTag();
        };

        self.addTagWithId = function (name, id) {
            if (name && id && !self.alreadyExists(name)) {
                var tagViewModel = new tags.TagViewModel(self, name, id);
                self.tags.push(tagViewModel);
            }
            self.clearTag();
        };

        self.alreadyExists = function (newTag) {
            for (var i = 0; i < self.tags().length; i++) {
                var tag = self.tags()[i];
                if (tag.name() == newTag) {
                    tag.isActive(true);
                    setTimeout(function () {
                        tag.isActive(false);
                    }, 4000);
                    self.clearTag();
                    return true;
                }
            }
            return false;
        };

        self.clearTag = function() {
            self.newTag('');
        };
    };
    
    /**
    * Tag view model
    */
    tags.TagViewModel = function (parent, tagName, tagId) {
        var self = this;

        self.parent = parent;
        self.pattern = 'Tags[{0}]';

        self.isActive = ko.observable(false);
        self.name = ko.observable(tagName);
        self.id = ko.observable(tagId);

        self.remove = function () {
            parent.tags.remove(self);
        };
        
        self.getTagInputName = function (index) {
            return $.format(self.pattern, index);
        };
    };

    function addTagAutoCompleteBinding() {
        ko.bindingHandlers.tagautocomplete = {
            init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
                var tagViewModel = viewModel,
                    onlyExisting = valueAccessor() == "onlyExisting",
                    complete = new autocomplete(element, {
                        serviceUrl: links.tagSuggestionSeviceUrl,
                        type: 'POST',
                        appendTo: $(element).parent(),
                        autoSelectFirst: onlyExisting,
                        transformResult: function(response) {
                            var result = typeof response === 'string' ? $.parseJSON(response) : response;
                            return {
                                suggestions: $.map(result.suggestions, function(dataItem) {
                                    return { value: dataItem.Value, data: dataItem.Key };
                                })
                            };
                        },
                        onSelect: function(suggestion) {
                            tagViewModel.newTag(suggestion.value);
                            if (onlyExisting) {
                                tagViewModel.addTagWithId(suggestion.value, suggestion.data);
                            } else {
                                tagViewModel.addTag();
                            }
                            tagViewModel.clearTag();
                        }
                    });
            }
        };
    }

    /**
    * Initializes tags module.
    */
    tags.init = function () {
        addTagAutoCompleteBinding();
        console.log('Initializing bcms.pages.tags module.');
    };
    
    /**
    * Register initialization
    */
    bcms.registerInit(tags.init);
    
    return tags;
});
