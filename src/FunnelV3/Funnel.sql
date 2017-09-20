SELECT DISTINCT Visits.Multisite AS 'Site', 'Home' AS 'Item Name', Pages.ItemId AS 'Item ID', COUNT(Pages.PageId) AS 'Visits', SUM(Visits.Value) AS 'Value', CONVERT(VARCHAR, Visits.StartDateTime, 107) AS 'Date' 
FROM Pages, Visits
WHERE Pages.VisitId = Visits.VisitId AND
Pages.ItemId = 'cb6ce567-ad2e-4828-9fae-a0a640516661'
GROUP BY Visits.Multisite, CONVERT(VARCHAR, Visits.StartDateTime, 107), Pages.ItemId

UNION SELECT DISTINCT Visits.Multisite, 'Campaign: ' + Campaigns.CampaignName, Pages.ItemId, COUNT(Pages.PageId), SUM(Visits.Value), CONVERT(VARCHAR, Visits.StartDateTime, 107) 
FROM PageEvents, Visits, Campaigns, Pages
WHERE Visits.VisitId = Pages.VisitId AND PageEvents.PageId = Pages.PageID AND
PageEvents.Data = '{'+CAST(Campaigns.CampaignId AS NVARCHAR(36))+'}' AND
PageEvents.Data in ('{<CAMPAIGN GUID 1>}','{<CAMPAIGN GUID N>}') 
GROUP BY Visits.Multisite, CONVERT(VARCHAR, Visits.StartDateTime, 107), 'Campaign: ' + Campaigns.CampaignName, Pages.ItemId

UNION SELECT DISTINCT Visits.Multisite, 'Event: ' + PageEventDefinitions.Name, A.PageEventDefinitionId, COUNT(A.PageEventId), SUM(Visits.Value), CONVERT(VARCHAR, Visits.StartDateTime, 107)
FROM Visits, PageEvents A, PageEvents B, PageEventDefinitions
WHERE A.VisitId = Visits.VisitId AND
A.PageEventDefinitionId = PageEventDefinitions.PageEventDefinitionId AND
B.VisitId = Visits.VisitId AND 
B.Data in ('{CAMPAIGN GUID 1}','{CAMPAIGN GUID N}') AND 
PageEventDefinitions.PageEventDefinitionId in ('<event guid 1>','<event guid N>')
GROUP BY Visits.Multisite, 'Event: ' + PageEventDefinitions.Name, A.PageEventDefinitionId, CONVERT(VARCHAR, Visits.StartDateTime, 107)

ORDER BY 'Site', 'Date', 'Item Name', 'Item ID'