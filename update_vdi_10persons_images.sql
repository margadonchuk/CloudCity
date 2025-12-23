-- Update VDI images for 10 persons products
-- Note: ProductType.VDI = 8 in the enum

-- Netherlands
UPDATE Products 
SET ImageUrl = '/images/vdi_start10_1.png'
WHERE Slug = 'vdi-start-netherlands-10' AND Configuration LIKE '%10 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt10_1.png'
WHERE Slug = 'vdi-standard-netherlands-10' AND Configuration LIKE '%10 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro10_1.png'
WHERE Slug = 'vdi-pro-netherlands-10' AND Configuration LIKE '%10 человек%';

-- Germany/France/Poland
UPDATE Products 
SET ImageUrl = '/images/vdi_start10_2.png'
WHERE Slug = 'vdi-start-europe-10' AND Configuration LIKE '%10 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt10_2.png'
WHERE Slug = 'vdi-standard-europe-10' AND Configuration LIKE '%10 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro10_2.png'
WHERE Slug = 'vdi-pro-europe-10' AND Configuration LIKE '%10 человек%';

-- USA/Canada/Asia
UPDATE Products 
SET ImageUrl = '/images/vdi_start10_3.png'
WHERE Slug = 'vdi-start-usa-asia-10' AND Configuration LIKE '%10 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt10_3.png'
WHERE Slug = 'vdi-standard-usa-asia-10' AND Configuration LIKE '%10 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro10_3.png'
WHERE Slug = 'vdi-pro-usa-asia-10' AND Configuration LIKE '%10 человек%';

